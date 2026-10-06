using System.Text;
using System.Text.Json;
using FluentAssertions;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Storage;
using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Storage;

namespace RepositorioRemoto.Tests.Storage;

[TestFixture]
public class UserStorageTests {

    public abstract class UserStorageTestsBase {

        protected UserStorage Storage = null!;
        protected string DirectorioTemporal = null!;
        protected string RutaArchivo = null!;
        protected User Usuario = null!;

        [SetUp]
        public void Setup() {
            Storage = new UserStorage();

            DirectorioTemporal = Path.Combine(
                Path.GetTempPath(),
                $"UserStorageTests_{Guid.NewGuid():N}"
            );

            Directory.CreateDirectory(DirectorioTemporal);

            RutaArchivo = Path.Combine(DirectorioTemporal, "users.json");

            Usuario = JsonSerializer.Deserialize<User>("""
                {
                    "id": 1,
                    "name": "Lucía García",
                    "userName": "lucia",
                    "email": "lucia@example.com",
                    "address": {
                        "street": "Calle Mayor",
                        "suite": "Piso 2",
                        "city": "Madrid",
                        "zipCode": "28001",
                        "geo": {
                            "lat": "40.4168",
                            "lng": "-3.7038"
                        }
                    },
                    "phone": "600123456",
                    "website": "lucia.example.com",
                    "company": {
                        "name": "Empresa de pruebas",
                        "catchPhrase": "Innovación y tecnología",
                        "bs": "desarrollo de software"
                    },
                    "createAt": "2026-10-06T10:00:00Z",
                    "updateAt": "2026-10-06T11:00:00Z",
                    "isDeleted": false
                }
                """,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }
            )!;
        }

        [TearDown]
        public void TearDown() {
            if (Directory.Exists(DirectorioTemporal)) {
                Directory.Delete(DirectorioTemporal, true);
            }
        }

        protected static void ComprobarErrorDeEscritura(DomainError error) {
            var errorEsperado = StorageErrors.WriteError("Detalle de prueba");

            error.Should().BeOfType(errorEsperado.GetType());
            error.Message.Should().NotBeNullOrWhiteSpace();
        }
    }

    [TestFixture]
    public sealed class CasosValidos : UserStorageTestsBase {

        [Test]
        public async Task ExportarJsonAsync_ConUsuarioValido_CreaArchivoConDatosCorrectos() {
            //Arrange
            var usuarios = new[] { Usuario };

            //Act
            var resultado = await Storage.ExportarJsonAsync(usuarios, RutaArchivo);

            //Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().BeTrue();
            File.Exists(RutaArchivo).Should().BeTrue();

            var contenido = await File.ReadAllTextAsync(RutaArchivo);

            using var documento = JsonDocument.Parse(contenido);
            var raiz = documento.RootElement;

            raiz.ValueKind.Should().Be(JsonValueKind.Array);
            raiz.GetArrayLength().Should().Be(1);

            var usuarioJson = raiz[0];

            usuarioJson.GetProperty("id").GetInt32().Should().Be(Usuario.Id);
            usuarioJson.GetProperty("name").GetString().Should().Be(Usuario.Name);
            usuarioJson.GetProperty("userName").GetString().Should().Be(Usuario.UserName);
            usuarioJson.GetProperty("email").GetString().Should().Be(Usuario.Email);
            usuarioJson.GetProperty("phone").GetString().Should().Be(Usuario.Phone);
            usuarioJson.GetProperty("website").GetString().Should().Be(Usuario.Website);
            usuarioJson.GetProperty("isDeleted").GetBoolean().Should().BeFalse();

            usuarioJson.GetProperty("createAt").GetDateTime().Should().Be(Usuario.CreateAt);
            usuarioJson.GetProperty("updateAt").GetDateTime().Should().Be(Usuario.UpdateAt);

            var address = usuarioJson.GetProperty("address");

            address.GetProperty("street").GetString().Should().Be(Usuario.Address.Street);
            address.GetProperty("suite").GetString().Should().Be(Usuario.Address.Suite);
            address.GetProperty("city").GetString().Should().Be(Usuario.Address.City);
            address.GetProperty("zipCode").GetString().Should().Be(Usuario.Address.ZipCode);

            var geo = address.GetProperty("geo");

            geo.GetProperty("lat").GetString().Should().Be(Usuario.Address.Geo.Lat);
            geo.GetProperty("lng").GetString().Should().Be(Usuario.Address.Geo.Lng);

            var company = usuarioJson.GetProperty("company");

            company.GetProperty("name").GetString().Should().Be(Usuario.Company.Name);
            company.GetProperty("catchPhrase").GetString().Should().Be(Usuario.Company.CatchPhrase);
            company.GetProperty("bs").GetString().Should().Be(Usuario.Company.Bs);
        }

        [Test]
        public async Task ExportarJsonAsync_ConVariosUsuarios_ExportaTodosEnOrden() {
            //Arrange
            var segundo = Usuario with {
                Id = 2,
                Name = "Carlos"
            };

            var usuarios = new[] { Usuario, segundo };

            //Act
            var resultado = await Storage.ExportarJsonAsync(usuarios, RutaArchivo);

            //Assert
            resultado.IsSuccess.Should().BeTrue();

            var contenido = await File.ReadAllTextAsync(RutaArchivo);

            using var documento = JsonDocument.Parse(contenido);
            var raiz = documento.RootElement;

            raiz.GetArrayLength().Should().Be(2);
            raiz[0].GetProperty("id").GetInt32().Should().Be(1);
            raiz[1].GetProperty("id").GetInt32().Should().Be(2);
            raiz[1].GetProperty("name").GetString().Should().Be("Carlos");
        }

        [Test]
        public async Task ExportarJsonAsync_ConColeccionVacia_EscribeArrayVacio() {
            //Arrange
            var usuarios = Array.Empty<User>();

            //Act
            var resultado = await Storage.ExportarJsonAsync(usuarios, RutaArchivo);

            //Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().BeTrue();

            var contenido = await File.ReadAllTextAsync(RutaArchivo);

            contenido.Should().Be("[]");
        }

        [Test]
        public async Task ExportarJsonAsync_ConDirectorioInexistente_CreaDirectoriosYArchivo() {
            //Arrange
            var ruta = Path.Combine(
                DirectorioTemporal,
                "exportaciones",
                "usuarios",
                "users.json"
            );

            var directorioDestino = Path.GetDirectoryName(ruta)!;

            //Act
            var resultado = await Storage.ExportarJsonAsync(new[] { Usuario }, ruta);

            //Assert
            resultado.IsSuccess.Should().BeTrue();
            Directory.Exists(directorioDestino).Should().BeTrue();
            File.Exists(ruta).Should().BeTrue();

            var contenido = await File.ReadAllTextAsync(ruta);

            using var documento = JsonDocument.Parse(contenido);
            documento.RootElement.GetArrayLength().Should().Be(1);
        }

        [Test]
        public async Task ExportarJsonAsync_ConRutaSinDirectorio_CreaArchivo() {
            //Arrange
            var ruta = $"users_test_{Guid.NewGuid():N}.json";

            try {
                //Act
                var resultado = await Storage.ExportarJsonAsync(new[] { Usuario }, ruta);

                //Assert
                resultado.IsSuccess.Should().BeTrue();
                File.Exists(ruta).Should().BeTrue();

                var contenido = await File.ReadAllTextAsync(ruta);

                using var documento = JsonDocument.Parse(contenido);
                documento.RootElement.GetArrayLength().Should().Be(1);
            } finally {
                File.Delete(ruta);
            }
        }

        [Test]
        public async Task ExportarJsonAsync_ConArchivoExistente_SobrescribeContenido() {
            //Arrange
            await File.WriteAllTextAsync(
                RutaArchivo,
                "Contenido anterior que debe desaparecer completamente."
            );

            //Act
            var resultado = await Storage.ExportarJsonAsync(
                Array.Empty<User>(),
                RutaArchivo
            );

            //Assert
            resultado.IsSuccess.Should().BeTrue();

            var contenido = await File.ReadAllTextAsync(RutaArchivo);

            contenido.Should().Be("[]");
        }

        [Test]
        public async Task ExportarJsonAsync_ConUsuario_UtilizaCamelCaseEIndentacion() {
            //Arrange
            var usuarios = new[] { Usuario };

            //Act
            var resultado = await Storage.ExportarJsonAsync(usuarios, RutaArchivo);

            //Assert
            resultado.IsSuccess.Should().BeTrue();

            var contenido = await File.ReadAllTextAsync(RutaArchivo);

            contenido.Should().Contain("\n");

            using var documento = JsonDocument.Parse(contenido);
            var usuarioJson = documento.RootElement[0];

            usuarioJson.TryGetProperty("userName", out _).Should().BeTrue();
            usuarioJson.TryGetProperty("createAt", out _).Should().BeTrue();

            usuarioJson.TryGetProperty("UserName", out _).Should().BeFalse();
            usuarioJson.TryGetProperty("CreateAt", out _).Should().BeFalse();
        }

        [Test]
        public async Task ExportarJsonAsync_ConPropiedadNula_OmitePropiedad() {
            //Arrange
            var usuario = Usuario with {
                Website = null!
            };

            //Act
            var resultado = await Storage.ExportarJsonAsync(new[] { usuario }, RutaArchivo);

            //Assert
            resultado.IsSuccess.Should().BeTrue();

            var contenido = await File.ReadAllTextAsync(RutaArchivo);

            using var documento = JsonDocument.Parse(contenido);
            var usuarioJson = documento.RootElement[0];

            usuarioJson.TryGetProperty("website", out _).Should().BeFalse();
            usuarioJson.GetProperty("name").GetString().Should().Be(usuario.Name);
        }

        [Test]
        public async Task ExportarJsonAsync_ConCaracteresEspeciales_EscribeUtf8SinBom() {
            //Arrange
            var usuario = Usuario with {
                Name = "Lucía, Peñalver & <Madrid>"
            };

            //Act
            var resultado = await Storage.ExportarJsonAsync(new[] { usuario }, RutaArchivo);

            //Assert
            resultado.IsSuccess.Should().BeTrue();

            var bytes = await File.ReadAllBytesAsync(RutaArchivo);
            var contenido = Encoding.UTF8.GetString(bytes);

            bytes.Take(3).Should().NotEqual(new byte[] { 0xEF, 0xBB, 0xBF });
            contenido.Should().Contain("Lucía, Peñalver & <Madrid>");

            using var documento = JsonDocument.Parse(contenido);

            documento.RootElement[0]
                .GetProperty("name")
                .GetString()
                .Should().Be(usuario.Name);
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos : UserStorageTestsBase {

        [Test]
        public async Task ExportarJsonAsync_ConColeccionNula_RetornaErrorDeEscritura() {
            //Arrange
            IEnumerable<User> usuarios = null!;

            //Act
            var resultado = await Storage.ExportarJsonAsync(usuarios, RutaArchivo);

            //Assert
            resultado.IsFailure.Should().BeTrue();
            ComprobarErrorDeEscritura(resultado.Error);
            File.Exists(RutaArchivo).Should().BeFalse();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\0")]
        public async Task ExportarJsonAsync_ConRutaInvalida_RetornaErrorDeEscritura(string? ruta) {
            //Arrange
            var usuarios = new[] { Usuario };

            //Act
            var resultado = await Storage.ExportarJsonAsync(usuarios, ruta!);

            //Assert
            resultado.IsFailure.Should().BeTrue();
            ComprobarErrorDeEscritura(resultado.Error);
        }

        [Test]
        public async Task ExportarJsonAsync_ConDirectorioComoDestino_RetornaErrorDeEscritura() {
            //Arrange
            var usuarios = new[] { Usuario };
            var ruta = DirectorioTemporal;

            //Act
            var resultado = await Storage.ExportarJsonAsync(usuarios, ruta);

            //Assert
            resultado.IsFailure.Should().BeTrue();
            ComprobarErrorDeEscritura(resultado.Error);
            Directory.Exists(DirectorioTemporal).Should().BeTrue();
        }

        [Test]
        public async Task ExportarJsonAsync_ConArchivoComoDirectorioPadre_RetornaErrorDeEscritura() {
            //Arrange
            var archivoBloqueante = Path.Combine(DirectorioTemporal, "bloqueante");

            await File.WriteAllTextAsync(archivoBloqueante, "Contenido original");

            var ruta = Path.Combine(archivoBloqueante, "users.json");

            //Act
            var resultado = await Storage.ExportarJsonAsync(new[] { Usuario }, ruta);

            //Assert
            resultado.IsFailure.Should().BeTrue();
            ComprobarErrorDeEscritura(resultado.Error);

            var contenido = await File.ReadAllTextAsync(archivoBloqueante);

            contenido.Should().Be("Contenido original");
        }

        [Test]
        public async Task ExportarJsonAsync_ConErrorAlEnumerar_RetornaErrorYConservaArchivo() {
            //Arrange
            const string contenidoOriginal = "[{\"id\":99}]";
            const string mensajeError = "Error al enumerar los usuarios.";

            await File.WriteAllTextAsync(RutaArchivo, contenidoOriginal);

            var usuarios = ObtenerUsuariosConError(Usuario, mensajeError);

            //Act
            var resultado = await Storage.ExportarJsonAsync(usuarios, RutaArchivo);

            //Assert
            resultado.IsFailure.Should().BeTrue();
            ComprobarErrorDeEscritura(resultado.Error);

            resultado.Error.Should().BeEquivalentTo(
                StorageErrors.WriteError(mensajeError)
            );

            var contenido = await File.ReadAllTextAsync(RutaArchivo);

            contenido.Should().Be(contenidoOriginal);
        }

        private static IEnumerable<User> ObtenerUsuariosConError(User usuario, string mensaje) {
            yield return usuario;

            throw new InvalidOperationException(mensaje);
        }
    }
}