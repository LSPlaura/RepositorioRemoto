using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RepositorioRemoto.Back.Entity;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Repositories;
using Testcontainers.PostgreSql;

namespace RepositorioRemoto.Tests.Repositories;

public abstract class UserEfcRepositoryPostgreTests
{
    protected static User CrearUsuarioBase(int id = 0, string name = "John Doe", bool isDeleted = false)
    {
        var geo = new Geo("40.7128", "-74.0060");
        var address = new Address("Kulas Light", "Apt. 556", "Gwenborough", "92998-3874", geo);
        var company = new Company("Romaguera-Crona", "Multi-layered client-server neural-net", "harness real-time e-markets");

        var ahora = DateTime.UtcNow;

        return new User(
            id,
            name,
            "johndoe",
            "john@example.com",
            address,
            "17707368031",
            "hildegard.org",
            company,
            ahora,
            ahora,
            ahora,
            isDeleted
        );
    }

    [TestFixture]
    public class CasosValidos : UserEfcRepositoryPostgreTests
    {
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:17-alpine")
            .WithCleanUp(true)
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private AppDbContextPostgre _context = null!;
        private UserEfcRepository _repository = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await _postgreSqlContainer.StartAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _postgreSqlContainer.DisposeAsync();
        }

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContextPostgre>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

            _context = new AppDbContextPostgre(options);
            await _context.Database.EnsureCreatedAsync();

            _repository = new UserEfcRepository(_context);
        }

        [TearDown]
        public async Task TearDown()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
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
            createResult.IsSuccess.Should().BeTrue();

            var realId = createResult.Value.Id;

            var result = await _repository.GetByIdAsync(realId);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(realId);
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
            var userOriginal = CrearUsuarioBase(name: "Nombre Original");
            var createResult = await _repository.CreateAsync(userOriginal);
            createResult.IsSuccess.Should().BeTrue();

            var id = createResult.Value.Id;

            _context.ChangeTracker.Clear();

            var userModificado = CrearUsuarioBase(id, "Nombre Actualizado");

            var result = await _repository.UpdateAsync(id, userModificado);

            _context.ChangeTracker.Clear();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Nombre Actualizado");

            var userInDb = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            userInDb.Should().NotBeNull();
            userInDb!.Name.Should().Be("Nombre Actualizado");
        }

        [Test]
        public async Task DeleteAsync_DebeEliminarUsuario_CuandoExisteYNoEstaBorrado()
        {
            var user = CrearUsuarioBase();
            var createResult = await _repository.CreateAsync(user);
            createResult.IsSuccess.Should().BeTrue();

            var id = createResult.Value.Id;

            var result = await _repository.DeleteAsync(id);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(id);

            var userInDb = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
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
    public class CasosInvalidosYExcepciones : UserEfcRepositoryPostgreTests
    {
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private AppDbContextPostgre _context = null!;
        private UserEfcRepository _repository = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await _postgreSqlContainer.StartAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _postgreSqlContainer.DisposeAsync();
        }

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContextPostgre>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

            _context = new AppDbContextPostgre(options);
            await _context.Database.EnsureCreatedAsync();

            _repository = new UserEfcRepository(_context);
        }

        [TearDown]
        public async Task TearDown()
        {
            try
            {
                if (_context != null)
                {
                    await _context.Database.EnsureDeletedAsync();
                    await _context.DisposeAsync();
                }
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
            var userBorrado = CrearUsuarioBase(id: 0, isDeleted: true);
            var createResult = await _repository.CreateAsync(userBorrado);
            createResult.IsSuccess.Should().BeTrue();

            var id = createResult.Value.Id;

            _context.ChangeTracker.Clear();

            var userActualizacion = CrearUsuarioBase(id, isDeleted: true);

            var result = await _repository.UpdateAsync(id, userActualizacion);

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
            var userBorrado = CrearUsuarioBase(id: 0, isDeleted: true);
            var createResult = await _repository.CreateAsync(userBorrado);
            createResult.IsSuccess.Should().BeTrue();

            var id = createResult.Value.Id;

            _context.ChangeTracker.Clear();

            var result = await _repository.DeleteAsync(id);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task CreateAsync_DebeRetornarFailure_CuandoOcurreExcepcionEnBD()
        {
            await _context.DisposeAsync();

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
            createResult.IsSuccess.Should().BeTrue();

            var id = createResult.Value.Id;

            await _context.DisposeAsync();

            var result = await _repository.UpdateAsync(id, user);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task DeleteAsync_DebeRetornarFailure_CuandoOcurreExcepcionEnBD()
        {
            var user = CrearUsuarioBase();
            var createResult = await _repository.CreateAsync(user);
            createResult.IsSuccess.Should().BeTrue();

            var id = createResult.Value.Id;

            await _context.DisposeAsync();

            var result = await _repository.DeleteAsync(id);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }

        [Test]
        public async Task DeleteAllAsync_DebeRetornarFailure_CuandoOcurreExcepcionEnBD()
        {
            await _context.DisposeAsync();

            var result = await _repository.DeleteAllAsync();

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeAssignableTo<DomainError>();
        }
    }
}