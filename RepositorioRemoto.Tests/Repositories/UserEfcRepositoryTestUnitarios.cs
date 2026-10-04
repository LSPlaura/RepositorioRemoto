using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using RepositorioRemoto.Back.Entity;
using RepositorioRemoto.Back.Errors.Repository;
using RepositorioRemoto.Back.Errors.Users;
using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Repositories;

namespace RepositorioRemoto.Tests.Repositories;

public class UserEfcRepositoryTestsUnitarios {

    public abstract class Base {

        protected Mock<AppDbContextPostgre> ContextMock = null!;
        protected Mock<DbSet<User>> UsersMock = null!;
        protected UserEfcRepository Repository = null!;

        protected List<User> Users = null!;

        [SetUp]
        public void SetUp() {
            var options = new DbContextOptions<AppDbContextPostgre>();

            ContextMock = new Mock<AppDbContextPostgre>(options);

            ContextMock
                .Setup(x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(1);

            Users = new List<User>();

            PrepararDbSet();

            Repository = new UserEfcRepository(
                ContextMock.Object
            );
        }

        protected void PrepararDbSet() {
            var queryable = Users.AsQueryable();

            UsersMock = new Mock<DbSet<User>>();

            UsersMock
                .As<IAsyncEnumerable<User>>()
                .Setup(x => x.GetAsyncEnumerator(
                    It.IsAny<CancellationToken>()
                ))
                .Returns(
                    new TestAsyncEnumerator<User>(
                        queryable.GetEnumerator()
                    )
                );

            UsersMock
                .As<IQueryable<User>>()
                .Setup(x => x.Provider)
                .Returns(
                    new TestAsyncQueryProvider<User>(
                        queryable.Provider
                    )
                );

            UsersMock
                .As<IQueryable<User>>()
                .Setup(x => x.Expression)
                .Returns(queryable.Expression);

            UsersMock
                .As<IQueryable<User>>()
                .Setup(x => x.ElementType)
                .Returns(queryable.ElementType);

            UsersMock
                .As<IQueryable<User>>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => queryable.GetEnumerator());

            UsersMock
                .Setup(x => x.FindAsync(
                    It.IsAny<object?[]?>()
                ))
                .Returns(
                    (object?[]? ids) => {
                        var id = (int)ids![0]!;

                        var user = Users
                            .FirstOrDefault(u => u.Id == id);

                        return new ValueTask<User?>(
                            user
                        );
                    }
                );

            ContextMock
                .Setup(x => x.Set<User>())
                .Returns(UsersMock.Object);
        }

        protected static User CreateUser(
            int id,
            bool isDeleted = false
        ) {
            return new User(
                Id: id,
                Name: $"Usuario {id}",
                UserName: $"usuario{id}",
                Email: $"usuario{id}@gmail.com",

                Address: new Address(
                    Street: "Calle Test",
                    Suite: "1A",
                    City: "Madrid",
                    ZipCode: "28001",

                    Geo: new Geo(
                        Lat: "40.4168",
                        Lng: "-3.7038"
                    )
                ),

                Phone: "600000000",
                Website: "test.com",

                Company: new Company(
                    Name: "Empresa Test",
                    CatchPhrase: "Empresa de pruebas",
                    Bs: "Testing"
                ),

                CreateAt: new DateTime(
                    2026,
                    1,
                    1
                ),

                UpdateAt: new DateTime(
                    2026,
                    1,
                    2
                ),

                DeleteAt: DateTime.MinValue,

                IsDeleted: isDeleted
            );
        }
    }

    [TestFixture]
    public class CasosValidos : Base {

        [Test]
        public async Task GetAllAsync_ExistenUsuarios_DevuelveUsuariosOrdenados() {
            //Arrange
            Users.AddRange([
                CreateUser(3),
                CreateUser(1),
                CreateUser(2)
            ]);

            PrepararDbSet();

            //Act
            var result = (
                await Repository.GetAllAsync()
            ).ToList();

            //Assert
            result.Should().HaveCount(3);

            result
                .Select(x => x.Id)
                .Should()
                .Equal(1, 2, 3);

            //Verify
            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task GetAllAsync_NoExistenUsuarios_DevuelveListaVacia() {
            //Arrange

            //Act
            var result =
                await Repository.GetAllAsync();

            //Assert
            result.Should().BeEmpty();

            //Verify
            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task GetByIdAsync_UsuarioExiste_DevuelveUsuario() {
            //Arrange
            var user = CreateUser(1);

            Users.Add(user);

            PrepararDbSet();

            //Act
            var result =
                await Repository.GetByIdAsync(1);

            //Assert
            result.IsSuccess.Should().BeTrue();

            result.Value
                .Should()
                .BeEquivalentTo(user);

            //Verify
            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task CreateAsync_UsuarioValido_DevuelveUsuario() {
            //Arrange
            var user = CreateUser(1);

            //Act
            var result =
                await Repository.CreateAsync(user);

            //Assert
            result.IsSuccess.Should().BeTrue();

            result.Value
                .Should()
                .BeEquivalentTo(user);

            //Verify
            UsersMock.Verify(
                x => x.Add(user),
                Times.Once
            );

            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task UpdateAsync_UsuarioExiste_DevuelveUsuarioActualizado() {
            //Arrange
            var user = CreateUser(1);

            Users.Add(user);

            PrepararDbSet();

            var updatedUser = user with {
                Name = "Usuario actualizado"
            };

            //Act
            var result =
                await Repository.UpdateAsync(
                    1,
                    updatedUser
                );

            //Assert
            result.IsSuccess.Should().BeTrue();

            result.Value
                .Should()
                .BeEquivalentTo(updatedUser);

            result.Value.Name
                .Should()
                .Be("Usuario actualizado");

            //Verify
            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task DeleteAsync_UsuarioExiste_EliminaUsuario() {
            //Arrange
            var user = CreateUser(1);

            Users.Add(user);

            PrepararDbSet();

            //Act
            var result =
                await Repository.DeleteAsync(1);

            //Assert
            result.IsSuccess.Should().BeTrue();

            result.Value
                .Should()
                .BeEquivalentTo(user);

            //Verify
            UsersMock.Verify(
                x => x.Remove(user),
                Times.Once
            );

            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task DeleteAllAsync_ExistenUsuarios_EliminaTodos() {
            //Arrange
            Users.AddRange([
                CreateUser(1),
                CreateUser(2),
                CreateUser(3)
            ]);

            PrepararDbSet();

            //Act
            var result =
                await Repository.DeleteAllAsync();

            //Assert
            result.IsSuccess.Should().BeTrue();

            result.Value
                .Should()
                .BeTrue();

            //Verify
            UsersMock.Verify(
                x => x.RemoveRange(
                    It.Is<IEnumerable<User>>(
                        users => users.Count() == 3
                    )
                ),
                Times.Once
            );

            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }
    }

    [TestFixture]
    public class CasosInvalidos : Base {

        [Test]
        public async Task GetByIdAsync_UsuarioNoExiste_DevuelveNotFound() {
            //Arrange
            const int id = 99;

            //Act
            var result =
                await Repository.GetByIdAsync(id);

            //Assert
            result.IsFailure.Should().BeTrue();

            result.Error
                .Should()
                .BeEquivalentTo(
                    UsersErrors.NotFoundError(id)
                );

            //Verify
            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task CreateAsync_ErrorAlGuardar_DevuelveCreationError() {
            //Arrange
            var user = CreateUser(1);

            ContextMock
                .Setup(x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ))
                .ThrowsAsync(
                    new Exception("Error")
                );

            //Act
            var result =
                await Repository.CreateAsync(user);

            //Assert
            result.IsFailure.Should().BeTrue();

            result.Error
                .Should()
                .BeEquivalentTo(
                    RepositoryErrors.CreationError()
                );

            //Verify
            UsersMock.Verify(
                x => x.Add(user),
                Times.Once
            );

            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task UpdateAsync_UsuarioNoExiste_DevuelveNotFound() {
            //Arrange
            const int id = 99;

            var updatedUser = CreateUser(id);

            //Act
            var result =
                await Repository.UpdateAsync(
                    id,
                    updatedUser
                );

            //Assert
            result.IsFailure.Should().BeTrue();

            result.Error
                .Should()
                .BeEquivalentTo(
                    UsersErrors.NotFoundError(id)
                );

            //Verify
            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task UpdateAsync_UsuarioBorrado_DevuelveUpdatedError() {
            //Arrange
            var user = CreateUser(
                1,
                true
            );

            Users.Add(user);

            PrepararDbSet();

            var updatedUser = user with {
                Name = "Usuario actualizado"
            };

            //Act
            var result =
                await Repository.UpdateAsync(
                    1,
                    updatedUser
                );

            //Assert
            result.IsFailure.Should().BeTrue();

            result.Error
                .Should()
                .BeEquivalentTo(
                    RepositoryErrors.UpdatedError()
                );

            //Verify
            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task UpdateAsync_ErrorAlGuardar_DevuelveUpdatedError() {
            //Arrange
            var user = CreateUser(1);

            Users.Add(user);

            PrepararDbSet();

            var updatedUser = user with {
                Name = "Usuario actualizado"
            };

            ContextMock
                .Setup(x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ))
                .ThrowsAsync(
                    new Exception("Error")
                );

            //Act
            var result =
                await Repository.UpdateAsync(
                    1,
                    updatedUser
                );

            //Assert
            result.IsFailure.Should().BeTrue();

            result.Error
                .Should()
                .BeEquivalentTo(
                    RepositoryErrors.UpdatedError()
                );

            //Verify
            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task DeleteAsync_UsuarioNoExiste_DevuelveNotFound() {
            //Arrange
            const int id = 99;

            //Act
            var result =
                await Repository.DeleteAsync(id);

            //Assert
            result.IsFailure.Should().BeTrue();

            result.Error
                .Should()
                .BeEquivalentTo(
                    UsersErrors.NotFoundError(id)
                );

            //Verify
            UsersMock.Verify(
                x => x.Remove(
                    It.IsAny<User>()
                ),
                Times.Never
            );

            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task DeleteAsync_UsuarioBorrado_NoEliminaUsuario() {
            //Arrange
            var user = CreateUser(
                1,
                true
            );

            Users.Add(user);

            PrepararDbSet();

            //Act
            var result =
                await Repository.DeleteAsync(1);

            //Assert
            result.IsFailure.Should().BeTrue();

            result.Error
                .Should()
                .BeEquivalentTo(
                    RepositoryErrors.UpdatedError()
                );

            //Verify
            UsersMock.Verify(
                x => x.Remove(
                    It.IsAny<User>()
                ),
                Times.Never
            );

            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [Test]
        public async Task DeleteAsync_ErrorAlGuardar_DevuelveDeletedError() {
            //Arrange
            var user = CreateUser(1);

            Users.Add(user);

            PrepararDbSet();

            ContextMock
                .Setup(x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ))
                .ThrowsAsync(
                    new Exception("Error")
                );

            //Act
            var result =
                await Repository.DeleteAsync(1);

            //Assert
            result.IsFailure.Should().BeTrue();

            result.Error
                .Should()
                .BeEquivalentTo(
                    RepositoryErrors.DeletedError()
                );

            //Verify
            UsersMock.Verify(
                x => x.Remove(user),
                Times.Once
            );

            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task DeleteAllAsync_ErrorAlGuardar_DevuelveDeletedError() {
            //Arrange
            Users.AddRange([
                CreateUser(1),
                CreateUser(2),
                CreateUser(3)
            ]);

            PrepararDbSet();

            ContextMock
                .Setup(x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ))
                .ThrowsAsync(
                    new Exception("Error")
                );

            //Act
            var result =
                await Repository.DeleteAllAsync();

            //Assert
            result.IsFailure.Should().BeTrue();

            result.Error
                .Should()
                .BeEquivalentTo(
                    RepositoryErrors.DeletedError()
                );

            //Verify
            UsersMock.Verify(
                x => x.RemoveRange(
                    It.Is<IEnumerable<User>>(
                        users => users.Count() == 3
                    )
                ),
                Times.Once
            );

            ContextMock.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }
    }

    private class TestAsyncQueryProvider<TEntity>
        : IAsyncQueryProvider {

        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(
            IQueryProvider inner
        ) {
            _inner = inner;
        }

        public IQueryable CreateQuery(
            Expression expression
        ) {
            return new TestAsyncEnumerable<TEntity>(
                expression
            );
        }

        public IQueryable<TElement> CreateQuery<TElement>(
            Expression expression
        ) {
            return new TestAsyncEnumerable<TElement>(
                expression
            );
        }

        public object? Execute(
            Expression expression
        ) {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(
            Expression expression
        ) {
            return _inner.Execute<TResult>(
                expression
            );
        }

        public TResult ExecuteAsync<TResult>(
            Expression expression,
            CancellationToken cancellationToken = default
        ) {
            var resultType =
                typeof(TResult)
                    .GetGenericArguments()[0];

            var result =
                typeof(IQueryProvider)
                    .GetMethods()
                    .First(
                        x =>
                            x.Name == "Execute" &&
                            x.IsGenericMethod
                    )
                    .MakeGenericMethod(resultType)
                    .Invoke(
                        _inner,
                        [expression]
                    );

            return (TResult)
                typeof(Task)
                    .GetMethod(
                        nameof(Task.FromResult)
                    )!
                    .MakeGenericMethod(resultType)
                    .Invoke(
                        null,
                        [result]
                    )!;
        }
    }

    private class TestAsyncEnumerable<T>
        : EnumerableQuery<T>,
            IAsyncEnumerable<T>,
            IQueryable<T> {
        
        public TestAsyncEnumerable(
            Expression expression
        ) : base(expression) {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(
            CancellationToken cancellationToken = default
        ) {
            return new TestAsyncEnumerator<T>(
                this
                    .AsEnumerable()
                    .GetEnumerator()
            );
        }

        IQueryProvider IQueryable.Provider =>
            new TestAsyncQueryProvider<T>(
                this
            );
    }

    private class TestAsyncEnumerator<T>
        : IAsyncEnumerator<T> {

        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(
            IEnumerator<T> inner
        ) {
            _inner = inner;
        }

        public T Current =>
            _inner.Current;

        public ValueTask<bool> MoveNextAsync() {
            return new ValueTask<bool>(
                _inner.MoveNext()
            );
        }

        public ValueTask DisposeAsync() {
            _inner.Dispose();

            return ValueTask.CompletedTask;
        }
    }
}