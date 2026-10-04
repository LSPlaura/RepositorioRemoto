using FluentAssertions;
using RepositorioRemoto.Back.Config;

namespace RepositorioRemoto.Tests.Config;

[TestFixture]
public class ApiConfigTests {

    [TestFixture]
    public sealed class Propiedades {

        [Test]
        public void BaseUrl_RetornaUrlPorDefecto() {
            // Arrange
            var config = new ApiConfig();

            // Act
            var baseUrl = config.BaseUrl;

            // Assert
            baseUrl.Should().NotBeNull();
            baseUrl.Should().Be("https://jsonplaceholder.typicode.com");
        }

        [Test]
        public void BaseUrl_RetornaUrlValida() {
            // Arrange
            var config = new ApiConfig();

            // Act
            var baseUrl = config.BaseUrl;
            var esValida = Uri.TryCreate(
                baseUrl,
                UriKind.Absolute,
                out var uri
            );

            // Assert
            esValida.Should().BeTrue();
            uri.Should().NotBeNull();
            uri!.Scheme.Should().Be(Uri.UriSchemeHttps);
        }

        [Test]
        public void BaseUrl_ModificarUrl_CambiaCorrectamente() {
            // Arrange
            var config = new ApiConfig();
            const string nuevaUrl = "https://localhost:5001";

            // Act
            config.BaseUrl = nuevaUrl;

            // Assert
            config.BaseUrl.Should().Be("https://localhost:5001");
        }
    }
}