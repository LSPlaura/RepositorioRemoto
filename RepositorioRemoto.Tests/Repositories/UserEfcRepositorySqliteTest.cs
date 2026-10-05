using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RepositorioRemoto.Back.Entity;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Repository;
using RepositorioRemoto.Back.Errors.Users;
using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Repositories;

namespace RepositorioRemoto.Tests.Repositories;

public abstract class UserEfcRepositoryTests
{
    protected const string InMemoryConnectionString = "Data Source=:memory:";

    protected static User CrearUsuarioBase(int id = 0, string name = "John Doe", bool isDeleted = false)
    {
        var geo = new Geo("40.7128", "-74.0060");
        var address = new Address("Kulas Light", "Apt. 556", "Gwenborough", "92998-3874", geo);
        var company = new Company("Romaguera-Crona", "Multi-layered client-server neural-net", "harness real-time e-markets");

        return new User(
            id,
            name,
            "johndoe",
            "john@example.com",
            address,
            "1-770-736-8031 x56442",
            "hildegard.org",
            company,
            DateTime.UtcNow,
            DateTime.UtcNow,
            DateTime.MinValue,
            isDeleted
        );
    }

    [TestFixture]
    public class CasosValidos : UserEfcRepositoryTests
    {
        private SqliteConnection _connection = null!;
        private AppDbContextPostgre _context = null!;
        private UserEfcRepository _repository = null!;

        [SetUp]
        public void SetUp()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContextPostgre>()
                .UseSqlite(_connection)
                .Options;

            _context = new AppDbContextPostgre(options);
            _context.Database.EnsureCreated();

            _repository = new UserEfcRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        [Test]
        public async Task GetAllAsync_DebeRetornarListaVacia_CuandoNoHayUsuarios()
        {
            var result = await _repository.GetAllAsync();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllAsync_DebeRetornarUsuariosOrdenadosPorId()
        {
            var user1 = CrearUsuarioBase(name: "User 1");
            var user2 = CrearUsuarioBase(name: "User 2");

            await _repository.CreateAsync(user2);
            await _repository.CreateAsync(user1);

            var result = (await _repository.GetAllAsync()).ToList();

            result.Should().HaveCount(2);
            result[0].Id.Should().BeLessThan(result[1].Id);
        }

        [Test]
        public async Task GetByIdAsync_DebeRetornarUsuario_CuandoExiste()
        {
            var user = CrearUsuarioBase();
            var createResult = await _repository.CreateAsync(user);

            var result = await _repository.GetByIdAsync(createResult.Value.Id);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(createResult.Value.Id);
            result.Value.Name.Should().Be("John Doe");
        }

        [Test]
        public async Task CreateAsync_DebeGuardarUsuarioCorrectamente()
        {
            var user = CrearUsuarioBase();

            var result = await _repository.CreateAsync(user);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().BeGreaterThan(0);
            result.Value.Name.Should().Be(user.Name);
        }

        [Test]
        public async Task UpdateAsync_DebeActualizarCamposDeUsuario_CuandoExisteYNoEstaBorrado()
        {
            var userOriginal = CrearUsuarioBase(name: "Original Name");
            var createResult = await _repository.CreateAsync(userOriginal);
            var id = createResult.Value.Id;

            _context.ChangeTracker.Clear();

            var userModificado = CrearUsuarioBase(id, name: "Updated Name");

            var result = await _repository.UpdateAsync(id, userModificado);

            _context.ChangeTracker.Clear();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Updated Name");

            var userInDb = await _context.Users.FindAsync(id);
            userInDb.Should().NotBeNull();
            userInDb!.Name.Should().Be("Updated Name");
        }

        [Test]
        public async Task DeleteAsync_DebeEliminarUsuario_CuandoExisteYNoEstaBorrado()
        {
            var user = CrearUsuarioBase();
            var createResult = await _repository.CreateAsync(user);
            var id = createResult.Value.Id;

            var result = await _repository.DeleteAsync(id);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(id);

            var userInDb = await _context.Users.FindAsync(id);
            userInDb.Should().BeNull();
        }

        [Test]
        public async Task DeleteAllAsync_DebeEliminarTodosLosUsuarios()
        {
            await _repository.CreateAsync(CrearUsuarioBase(name: "User 1"));
            await _repository.CreateAsync(CrearUsuarioBase(name: "User 2"));

            var result = await _repository.DeleteAllAsync();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            var allUsers = await _repository.GetAllAsync();
            allUsers.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class CasosInvalidosYExcepciones : UserEfcRepositoryTests
    {
        private SqliteConnection _connection = null!;
        private AppDbContextPostgre _context = null!;
        private UserEfcRepository _repository = null!;

        [SetUp]
        public void SetUp()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContextPostgre>()
                .UseSqlite(_connection)
                .Options;

            _context = new AppDbContextPostgre(options);
            _context.Database.EnsureCreated();

            _repository = new UserEfcRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _context?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
            }
            catch (ObjectDisposedException)
            {
                
            }
        }

        [Test]
        public async Task GetByIdAsync_DebeRetornarFailure_CuandoUsuarioNoExiste()
        {
            var result = await _repository.GetByIdAsync(999);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task UpdateAsync_DebeRetornarFailure_CuandoUsuarioNoExiste()
        {
            var user = CrearUsuarioBase();

            var result = await _repository.UpdateAsync(999, user);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task UpdateAsync_DebeRetornarFailure_CuandoUsuarioEstaBorrado()
        {
            var userBorrado = CrearUsuarioBase(isDeleted: true);
            var createResult = await _repository.CreateAsync(userBorrado);

            var result = await _repository.UpdateAsync(createResult.Value.Id, userBorrado);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task DeleteAsync_DebeRetornarFailure_CuandoUsuarioNoExiste()
        {
            var result = await _repository.DeleteAsync(999);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task DeleteAsync_DebeRetornarFailure_CuandoUsuarioEstaBorrado()
        {
            var userBorrado = CrearUsuarioBase(isDeleted: true);
            var createResult = await _repository.CreateAsync(userBorrado);

            var result = await _repository.DeleteAsync(createResult.Value.Id);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }
        
        [Test]
        public async Task CreateAsync_DebeRetornarFailure_CuandoOcurreExcepcionEnBD()
        {
            _connection.Close();

            var user = CrearUsuarioBase();
            var result = await _repository.CreateAsync(user);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task UpdateAsync_DebeRetornarFailure_CuandoOcurreExcepcionEnBD()
        {
            var user = CrearUsuarioBase();
            var createResult = await _repository.CreateAsync(user);

            _connection.Close();

            var result = await _repository.UpdateAsync(createResult.Value.Id, user);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task DeleteAsync_DebeRetornarFailure_CuandoOcurreExcepcionEnBD()
        {
            var user = CrearUsuarioBase();
            var createResult = await _repository.CreateAsync(user);

            _connection.Close();

            var result = await _repository.DeleteAsync(createResult.Value.Id);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task DeleteAllAsync_DebeRetornarFailure_CuandoOcurreExcepcionEnBD()
        {
            _connection.Close();

            var result = await _repository.DeleteAllAsync();

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }
    }
}