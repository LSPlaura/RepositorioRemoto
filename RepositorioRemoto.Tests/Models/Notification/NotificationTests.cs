using FluentAssertions;
using RepositorioRemoto.Back.Enum;

namespace RepositorioRemoto.Tests.Models.Notification;

[TestFixture]
public class NotificationTests {

    [TestFixture]
    public sealed class CasosValidos {

        public static IEnumerable<TypeNotification> TiposValidos =>
            System.Enum.GetValues<TypeNotification>();

        [TestCaseSource(nameof(TiposValidos))]
        public void Constructor_ConDatosValidos_AsignaPropiedades(TypeNotification tipo) {
            //Arrange
            const string mensaje = "Operación realizada correctamente.";
            var timestamp = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc);

            //Act
            var resultado = new RepositorioRemoto.Back.Models.Notification.Notification(
                tipo,
                mensaje,
                timestamp
            );

            //Assert
            resultado.Tipo.Should().Be(tipo);
            resultado.Mensaje.Should().Be(mensaje);
            resultado.Timestamp.Should().Be(timestamp);
            resultado.Timestamp.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Test]
        public void Equals_ConMismosDatos_RetornaTrue() {
            //Arrange
            var tipo = default(TypeNotification);
            const string mensaje = "Usuario creado.";
            var timestamp = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc);

            var primera = new RepositorioRemoto.Back.Models.Notification.Notification(
                tipo, mensaje, timestamp
            );

            var segunda = new RepositorioRemoto.Back.Models.Notification.Notification(
                tipo, mensaje, timestamp
            );

            //Act
            var resultado = primera.Equals(segunda);

            //Assert
            resultado.Should().BeTrue();
            primera.Should().NotBeSameAs(segunda);
        }

        [Test]
        public void OperadoresIgualdad_ConMismosDatos_ComparanPorValor() {
            //Arrange
            var timestamp = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc);

            var primera = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Usuario creado.", timestamp
            );

            var segunda = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Usuario creado.", timestamp
            );

            //Act
            var iguales = primera == segunda;
            var diferentes = primera != segunda;

            //Assert
            iguales.Should().BeTrue();
            diferentes.Should().BeFalse();
        }

        [Test]
        public void Equals_ConMensajeDistinto_RetornaFalse() {
            //Arrange
            var timestamp = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc);

            var primera = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Usuario creado.", timestamp
            );

            var segunda = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Usuario eliminado.", timestamp
            );

            //Act
            var resultado = primera.Equals(segunda);

            //Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void Equals_ConTimestampDistinto_RetornaFalse() {
            //Arrange
            var timestamp = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc);

            var primera = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Usuario creado.", timestamp
            );

            var segunda = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Usuario creado.", timestamp.AddSeconds(1)
            );

            //Act
            var resultado = primera.Equals(segunda);

            //Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void GetHashCode_ConMismosDatos_RetornaMismoHash() {
            //Arrange
            var timestamp = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc);

            var primera = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Usuario creado.", timestamp
            );

            var segunda = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Usuario creado.", timestamp
            );

            //Act
            var primerHash = primera.GetHashCode();
            var segundoHash = segunda.GetHashCode();

            //Assert
            primerHash.Should().Be(segundoHash);
        }

        [Test]
        public void Deconstruct_ConNotificacion_ExtraePropiedades() {
            //Arrange
            var tipoEsperado = default(TypeNotification);
            const string mensajeEsperado = "Usuario actualizado.";
            var timestampEsperado = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc);

            var notificacion = new RepositorioRemoto.Back.Models.Notification.Notification(
                tipoEsperado,
                mensajeEsperado,
                timestampEsperado
            );

            //Act
            var (tipo, mensaje, timestamp) = notificacion;

            //Assert
            tipo.Should().Be(tipoEsperado);
            mensaje.Should().Be(mensajeEsperado);
            timestamp.Should().Be(timestampEsperado);
        }

        [Test]
        public void With_AlCambiarMensaje_CreaCopiaSinModificarOriginal() {
            //Arrange
            var timestamp = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc);

            var original = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Mensaje original.", timestamp
            );

            //Act
            var copia = original with {
                Mensaje = "Mensaje actualizado."
            };

            //Assert
            copia.Should().NotBeSameAs(original);
            copia.Mensaje.Should().Be("Mensaje actualizado.");
            copia.Tipo.Should().Be(original.Tipo);
            copia.Timestamp.Should().Be(original.Timestamp);
            original.Mensaje.Should().Be("Mensaje original.");
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_ConMensajeNuloVacioOBlanco_ConservaValor(string? mensaje) {
            //Arrange
            var timestamp = new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc);

            //Act
            var resultado = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, mensaje!, timestamp
            );

            //Assert
            resultado.Mensaje.Should().Be(mensaje);
            resultado.Timestamp.Should().Be(timestamp);
        }

        [Test]
        public void Constructor_ConTimestampPorDefecto_ConservaValor() {
            //Arrange
            var timestamp = default(DateTime);

            //Act
            var resultado = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Mensaje de prueba.", timestamp
            );

            //Assert
            resultado.Timestamp.Should().Be(default(DateTime));
        }

        [Test]
        public void Equals_ConNull_RetornaFalse() {
            //Arrange
            var notificacion = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Mensaje de prueba.", DateTime.UtcNow
            );

            //Act
            var resultado = notificacion.Equals(null);

            //Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void Equals_ConObjetoDeOtroTipo_RetornaFalse() {
            //Arrange
            var notificacion = new RepositorioRemoto.Back.Models.Notification.Notification(
                default, "Mensaje de prueba.", DateTime.UtcNow
            );

            object otroObjeto = "Mensaje de prueba.";

            //Act
            var resultado = notificacion.Equals(otroObjeto);

            //Assert
            resultado.Should().BeFalse();
        }
    }
}