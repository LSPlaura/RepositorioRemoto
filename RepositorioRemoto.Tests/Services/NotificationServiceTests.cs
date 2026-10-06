using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepositorioRemoto.Back.Enum;
using RepositorioRemoto.Back.Models.Notification;
using RepositorioRemoto.Back.Services.Notifications;

namespace RepositorioRemoto.Tests.Services;

public abstract class NotificationServiceTests
{
    [TestFixture]
    public class CasosValidos
    {
        private ConsoleNotificationService _service = null!;
        private Mock<IObserver<Notification>> _observer = null!;
        private IDisposable _subscription = null!;

        [SetUp]
        public void SetUp()
        {
            _service = new ConsoleNotificationService();
            _observer = new Mock<IObserver<Notification>>();
            _subscription = _service.Observable.Subscribe(_observer.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _subscription.Dispose();
            _service.Dispose();
            _observer.Reset();
        }

        [Test]
        public void Notificar_DebePublicarLaNotificacion()
        {
            var notificacion = new Notification(TypeNotification.Create, "Usuario creado", DateTime.UtcNow);

            _service.Notificar(notificacion);

            _observer.Verify(observer => observer.OnNext(notificacion), Times.Once);
        }
    }

    [TestFixture]
    public class CasosInvalidos
    {
        private ConsoleNotificationService _service = null!;
        private Mock<IObserver<Notification>> _observer = null!;
        private IDisposable _subscription = null!;

        [SetUp]
        public void SetUp()
        {
            _service = new ConsoleNotificationService();
            _observer = new Mock<IObserver<Notification>>();
            _subscription = _service.Observable.Subscribe(_observer.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _subscription.Dispose();
            _service.Dispose();
            _observer.Reset();
        }

        [Test]
        public void Notificar_DebePublicarElValorNuloSinPerderElEvento()
        {
            _service.Notificar(null!);

            _observer.Verify(observer => observer.OnNext(null!), Times.Once);
        }
    }
}
