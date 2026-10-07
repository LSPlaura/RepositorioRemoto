using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepositorioRemoto.Back.Api;
using RepositorioRemoto.Back.Cache.Common;
using RepositorioRemoto.Back.Config;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Models.Notification;
using RepositorioRemoto.Back.Notifications;
using RepositorioRemoto.Back.Repositories;
using RepositorioRemoto.Back.Services.User;
using RepositorioRemoto.Back.Storage;
using RepositorioRemoto.Back.Validator;

namespace RepositorioRemoto.Tests.Services;

public abstract class UserServiceTests
{
    [TestFixture]
    public class CasosValidos
    {
        private Mock<IValidate<User>> _validator = null!;
        private Mock<IUserRepository> _repository = null!;
        private Mock<ICache> _cache = null!;
        private Mock<IUserStorage> _storage = null!;
        private Mock<INotificationService> _notifications = null!;
        private Mock<IApiJsonPlaceHolder> _api = null!;
        private UserService _service = null!;

        [SetUp]
        public void SetUp() => CreateService();

        [TearDown]
        public void TearDown() => ResetMocks();

        [Test]
        public async Task GetAllAsync_DebeUsarElRepositorioSinConsultarLaApi_CuandoHayUsuarios()
        {
            var users = new[] { User(1) };
            _repository.Setup(x => x.GetAllAsync()).ReturnsAsync(users);

            var result = await _service.GetAllAsync();

            result.Should().BeEquivalentTo(users);
            _api.Verify(x => x.GetUserAsync(), Times.Never);
        }

        [Test]
        public async Task GetAllAsync_DebeConsultarApi_CuandoElRepositorioEstaVacio()
        {
            var users = new List<User> { User(1), User(2) };
            _repository.Setup(x => x.GetAllAsync()).ReturnsAsync(Array.Empty<User>());
            _api.Setup(x => x.GetUserAsync()).ReturnsAsync(users);

            var result = await _service.GetAllAsync();

            result.Should().BeEquivalentTo(users);
            _api.Verify(x => x.GetUserAsync(), Times.Once);
            _repository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task GetByIdAsync_DebeDevolverUsuarioDesdeCache()
        {
            var user = User(7);
            _cache.Setup(x => x.GetAsync<User>("User:7")).ReturnsAsync(user);

            var result = await _service.GetByIdAsync(7);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(user);
            _repository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetByIdAsync_DebeDevolverUsuarioDelRepositorioYCachearlo()
        {
            var user = User(7);
            _cache.Setup(x => x.GetAsync<User>("User:7")).ReturnsAsync((User?)null);
            _repository.Setup(x => x.GetByIdAsync(7)).ReturnsAsync(Success(user));

            var result = await _service.GetByIdAsync(7);

            result.Value.Should().BeEquivalentTo(user);
            _cache.Verify(x => x.SetAsync("User:7", user, null), Times.Once);
            _api.Verify(x => x.GetUserByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetByIdAsync_DebeGuardarEnRepositorioYCache_CuandoSoloExisteEnApi()
        {
            var user = User(7);
            _cache.Setup(x => x.GetAsync<User>("User:7")).ReturnsAsync((User?)null);
            _repository.Setup(x => x.GetByIdAsync(7)).ReturnsAsync(Failure<User>());
            _api.Setup(x => x.GetUserByIdAsync(7)).ReturnsAsync(user);
            _repository.Setup(x => x.CreateAsync(user)).ReturnsAsync(Success(user));

            var result = await _service.GetByIdAsync(7);

            result.Value.Should().BeEquivalentTo(user);
            _repository.Verify(x => x.CreateAsync(user), Times.Once);
            _cache.Verify(x => x.SetAsync("User:7", user, null), Times.Once);
        }

        [Test]
        public async Task CreateAsync_DebeValidarCrearYNotificar()
        {
            var request = Request();
            var created = User(12);
            ValidModel();
            _api.Setup(x => x.CreateUserAsync(request)).ReturnsAsync(created);
            _repository.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync((User user) => Success(user));

            var result = await _service.CreateAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(12);
            _notifications.Verify(x => x.Notificar(It.Is<Notification>(n => n.Tipo == RepositorioRemoto.Back.Enum.TypeNotification.Create)), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_DebeActualizarYNotificar()
        {
            var request = UpdateRequest(4);
            var existing = User(4);
            ValidModel();
            _cache.Setup(x => x.GetAsync<User>("User:4")).ReturnsAsync(existing);
            _api.Setup(x => x.UpdateUserAsync(4, request)).ReturnsAsync(existing);
            _repository.Setup(x => x.UpdateAsync(4, It.IsAny<User>())).ReturnsAsync(Success(existing));

            var result = await _service.UpdateAsync(4, request);

            result.IsSuccess.Should().BeTrue();
            _api.Verify(x => x.UpdateUserAsync(4, request), Times.Once);
            _notifications.Verify(x => x.Notificar(It.Is<Notification>(n => n.Tipo == RepositorioRemoto.Back.Enum.TypeNotification.Update)), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_DebeEliminarEnApiRepositorioCacheYNotificar()
        {
            _cache.Setup(x => x.GetAsync<User>("User:4")).ReturnsAsync(User(4));
            _api.Setup(x => x.DeleteUserAsync(4)).Returns(Task.CompletedTask);
            _repository.Setup(x => x.DeleteAsync(4)).ReturnsAsync(Success(User(4)));

            var result = await _service.DeleteAsync(4);

            result.IsSuccess.Should().BeTrue();
            _api.Verify(x => x.DeleteUserAsync(4), Times.Once);
            _repository.Verify(x => x.DeleteAsync(4), Times.Once);
            _cache.Verify(x => x.RemoveAsync("User:4"), Times.Once);
            _notifications.Verify(x => x.Notificar(It.Is<Notification>(n => n.Tipo == RepositorioRemoto.Back.Enum.TypeNotification.Delete)), Times.Once);
        }

        [Test]
        public async Task ExportToJsonAsync_DebeExportarYDevolverTrue()
        {
            var users = new[] { User(1) };
            _repository.Setup(x => x.GetAllAsync()).ReturnsAsync(users);
            _storage.Setup(x => x.ExportarJsonAsync(users, Configuracion.UsersJsonPath))
                .ReturnsAsync(Result.Success<bool, DomainError>(true));

            var result = await _service.ExportToJsonAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        private void CreateService()
        {
            _validator = new Mock<IValidate<User>>();
            _repository = new Mock<IUserRepository>();
            _cache = new Mock<ICache>();
            _storage = new Mock<IUserStorage>();
            _notifications = new Mock<INotificationService>();
            _api = new Mock<IApiJsonPlaceHolder>();
            _service = new UserService(_validator.Object, _repository.Object, _cache.Object, _storage.Object, _notifications.Object, _api.Object);
        }

        private void ResetMocks()
        {
            _validator.Reset();
            _repository.Reset();
            _cache.Reset();
            _storage.Reset();
            _notifications.Reset();
            _api.Reset();
        }

        private void ValidModel() => _validator.Setup(x => x.Validate(It.IsAny<User>())).Returns(Result.Success<bool, DomainError>(true));
    }

    [TestFixture]
    public class CasosInvalidos
    {
        private Mock<IValidate<User>> _validator = null!;
        private Mock<IUserRepository> _repository = null!;
        private Mock<ICache> _cache = null!;
        private Mock<IUserStorage> _storage = null!;
        private Mock<INotificationService> _notifications = null!;
        private Mock<IApiJsonPlaceHolder> _api = null!;
        private UserService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _validator = new Mock<IValidate<User>>();
            _repository = new Mock<IUserRepository>();
            _cache = new Mock<ICache>();
            _storage = new Mock<IUserStorage>();
            _notifications = new Mock<INotificationService>();
            _api = new Mock<IApiJsonPlaceHolder>();
            _service = new UserService(_validator.Object, _repository.Object, _cache.Object, _storage.Object, _notifications.Object, _api.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _validator.Reset();
            _repository.Reset();
            _cache.Reset();
            _storage.Reset();
            _notifications.Reset();
            _api.Reset();
        }

        [Test]
        public async Task GetByIdAsync_DebeNotFound_CuandoNoExisteEnNingunaFuente()
        {
            _repository.Setup(x => x.GetByIdAsync(99)).ReturnsAsync(Failure<User>());
            _api.Setup(x => x.GetUserByIdAsync(99)).ReturnsAsync((User?)null);

            var result = await _service.GetByIdAsync(99);

            result.IsFailure.Should().BeTrue();
            result.Error.Message.Should().Contain("99");
        }

        [Test]
        public async Task GetByIdAsync_DebeConvertirExcepcionEnServiceError()
        {
            _cache.Setup(x => x.GetAsync<User>("User:1")).ThrowsAsync(new InvalidOperationException());

            var result = await _service.GetByIdAsync(1);

            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public async Task CreateAsync_DebeFallar_CuandoRequestEsNulo()
        {
            var result = await _service.CreateAsync(null!);

            result.IsFailure.Should().BeTrue();
            _api.Verify(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>()), Times.Never);
        }

        [Test]
        public async Task CreateAsync_DebeFallar_CuandoValidacionFalla()
        {
            _validator.Setup(x => x.Validate(It.IsAny<User>()))
                .Returns(Result.Failure<bool, DomainError>(new TestError("invalid")));

            var result = await _service.CreateAsync(Request());

            result.IsFailure.Should().BeTrue();
            _api.Verify(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>()), Times.Never);
        }

        [Test]
        public async Task CreateAsync_DebeCapturarExcepcionDeApi()
        {
            _validator.Setup(x => x.Validate(It.IsAny<User>())).Returns(Result.Success<bool, DomainError>(true));
            _api.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ThrowsAsync(new Exception());

            var result = await _service.CreateAsync(Request());

            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public async Task UpdateAsync_DebeFallar_CuandoIdsNoCoinciden()
        {
            var result = await _service.UpdateAsync(1, UpdateRequest(2));

            result.IsFailure.Should().BeTrue();
            _api.Verify(x => x.UpdateUserAsync(It.IsAny<int>(), It.IsAny<UpdateUserRequest>()), Times.Never);
        }

        [Test]
        public async Task UpdateAsync_DebeFallar_CuandoValidacionEsInvalida()
        {
            _validator.Setup(x => x.Validate(It.IsAny<User>()))
                .Returns(Result.Failure<bool, DomainError>(new TestError("invalid")));

            var result = await _service.UpdateAsync(1, UpdateRequest(1));

            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public async Task UpdateAsync_DebeFallar_CuandoUsuarioNoExiste()
        {
            _validator.Setup(x => x.Validate(It.IsAny<User>())).Returns(Result.Success<bool, DomainError>(true));
            _repository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Failure<User>());
            _api.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync((User?)null);

            var result = await _service.UpdateAsync(1, UpdateRequest(1));

            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public async Task UpdateAsync_DebeCapturarExcepcion()
        {
            _validator.Setup(x => x.Validate(It.IsAny<User>())).Throws(new InvalidOperationException());

            var result = await _service.UpdateAsync(1, UpdateRequest(1));

            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public async Task DeleteAsync_DebeFallar_CuandoUsuarioNoExiste()
        {
            _repository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Failure<User>());
            _api.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync((User?)null);

            var result = await _service.DeleteAsync(1);

            result.IsFailure.Should().BeTrue();
            _repository.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_DebeCapturarExcepcion()
        {
            _cache.Setup(x => x.GetAsync<User>("User:1")).ThrowsAsync(new Exception());

            var result = await _service.DeleteAsync(1);

            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public async Task ExportToJsonAsync_DebeDevolverFallo_CuandoStorageFalla()
        {
            var users = new[] { User(1) };
            _repository.Setup(x => x.GetAllAsync()).ReturnsAsync(users);
            _storage.Setup(x => x.ExportarJsonAsync(users, Configuracion.UsersJsonPath))
                .ReturnsAsync(Result.Failure<bool, DomainError>(new TestError("write")));

            var result = await _service.ExportToJsonAsync();

            result.IsFailure.Should().BeTrue();
        }

        private sealed record TestError(string Message) : DomainError(Message);
    }

    private static User User(int id) => new(id, "Laura", "laura", "laura@example.com", new Address(), "600000000", "example.com", new Company(), DateTime.UtcNow, DateTime.UtcNow, default, false);
    private static CreateUserRequest Request() => new("Laura", "laura", "laura@example.com", new AddressDto("", "", "", "", new GeoDto("", "")), "600000000", "example.com", new CompanyDto("", "", ""));
    private static UpdateUserRequest UpdateRequest(int id) => new(id, "Laura", "laura", "laura@example.com", new AddressDto("", "", "", "", new GeoDto("", "")), "600000000", "example.com", new CompanyDto("", "", ""));
    private static Result<User, DomainError> Success(User user) => Result.Success<User, DomainError>(user);
    private static Result<User, DomainError> Failure<User>() => Result.Failure<User, DomainError>(new TestFailure("missing"));
    private sealed record TestFailure(string Message) : DomainError(Message);
}
