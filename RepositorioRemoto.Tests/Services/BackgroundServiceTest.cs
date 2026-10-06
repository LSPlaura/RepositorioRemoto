using FluentAssertions;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using RepositorioRemoto.Back.Cache.Common;
using RepositorioRemoto.Back.Api;
using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Repositories;
using RepositorioRemoto.Back.Services.Background;
using System.Reflection;

namespace RepositorioRemoto.Tests.Services;

public abstract class BackgroundServiceTest
{
    [TestFixture]
    public class CasosValidos
    {
        private Mock<IServiceProvider> _provider = null!;
        private Mock<ICache> _cache = null!;
        private BackgroundService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _provider = new Mock<IServiceProvider>();
            _cache = new Mock<ICache>();
            _service = new BackgroundService(_provider.Object, _cache.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _provider.Reset();
            _cache.Reset();
        }

        [Test]
        public async Task StartAsync_DebeFinalizarCuandoElTokenYaEstaCancelado()
        {
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            var act = () => _service.StartAsync(cancellation.Token);

            await act.Should().NotThrowAsync();
            _cache.Verify(cache => cache.RemoveAllAsync(), Times.Never);
        }

        [Test]
        public async Task Synchronize_DebeLimpiarCacheYRepositorioYCrearCadaUsuario()
        {
            var repository = new Mock<IUserRepository>();
            var api = new Mock<IApiJsonPlaceHolder>();
            var users = new List<User> { User(1), User(2) };
            api.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            repository.Setup(x => x.DeleteAllAsync()).ReturnsAsync(Result.Success<bool, RepositorioRemoto.Back.Errors.DomainError>(true));
            _cache.Setup(x => x.RemoveAllAsync()).Returns(Task.CompletedTask);
            repository.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => Result.Success<User, RepositorioRemoto.Back.Errors.DomainError>(user));

            await InvokeSynchronize(repository.Object, api.Object);

            _cache.Verify(x => x.RemoveAllAsync(), Times.Once);
            repository.Verify(x => x.DeleteAllAsync(), Times.Once);
            api.Verify(x => x.GetUserAsync(), Times.Once);
            repository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Exactly(users.Count));
        }

        private async Task InvokeSynchronize(IUserRepository repository, IApiJsonPlaceHolder api)
        {
            var method = typeof(BackgroundService).GetMethod("Synchronize", BindingFlags.Instance | BindingFlags.NonPublic)!;
            await (Task)method.Invoke(_service, new object[] { repository, api })!;
        }

        private static User User(int id) => new(id, "Laura", "laura", "laura@example.com",
            new Address(), "600000000", "example.com", new Company(), DateTime.UtcNow, DateTime.UtcNow, default, false);
    }

    [TestFixture]
    public class CasosInvalidos
    {
        private Mock<IServiceProvider> _provider = null!;
        private Mock<ICache> _cache = null!;
        private BackgroundService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _provider = new Mock<IServiceProvider>();
            _cache = new Mock<ICache>();
            _service = new BackgroundService(_provider.Object, _cache.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _provider.Reset();
            _cache.Reset();
        }

        [Test]
        public async Task StartAsync_DebeCancelarSinLanzarExcepcion_CuandoSeSolicitaCancelacion()
        {
            using var cancellation = new CancellationTokenSource();
            var task = _service.StartAsync(cancellation.Token);
            cancellation.Cancel();

            Func<Task> act = () => task;
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task Synchronize_DebePropagarExcepcionDeLaApi()
        {
            var repository = new Mock<IUserRepository>();
            var api = new Mock<IApiJsonPlaceHolder>();
            api.Setup(x => x.GetUserAsync()).ThrowsAsync(new InvalidOperationException("API caída"));

            var method = typeof(BackgroundService).GetMethod("Synchronize", BindingFlags.Instance | BindingFlags.NonPublic)!;
            Func<Task> act = async () => await (Task)method.Invoke(_service, new object[] { repository.Object, api.Object })!;

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
