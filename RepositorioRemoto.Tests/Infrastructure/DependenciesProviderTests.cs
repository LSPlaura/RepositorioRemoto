using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Back.Infrastructure;

namespace RepositorioRemoto.Tests.Infrastructure;

[TestFixture]
public class DependenciesProviderTests {

    [TestFixture]
    public sealed class CasosValidos {

        [Test]
        public void ServicesProvider_ConfiguracionCorrecta_RetornaServiceProvider() {
            // Arrange

            // Act
            var provider = DependenciesProvider.ServicesProvider();

            // Assert
            provider.Should().NotBeNull();
            provider.Should().BeAssignableTo<IServiceProvider>();
        }

        [Test]
        public void ServicesProvider_DosLlamadas_RetornanProvidersDiferentes() {
            // Arrange

            // Act
            var provider1 = DependenciesProvider.ServicesProvider();
            var provider2 = DependenciesProvider.ServicesProvider();

            // Assert
            provider1.Should().NotBeNull();
            provider2.Should().NotBeNull();

            provider1.Should().NotBeSameAs(provider2);
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        [Test]
        public void ServicioNoRegistrado_GetService_RetornaNull() {
            // Arrange
            var provider = DependenciesProvider.ServicesProvider();

            // Act
            var service = provider.GetService<IServicioNoRegistrado>();

            // Assert
            service.Should().BeNull();
        }

        [Test]
        public void ServicioNoRegistrado_GetRequiredService_LanzaExcepcion() {
            // Arrange
            var provider = DependenciesProvider.ServicesProvider();

            // Act
            var action = () =>
                provider.GetRequiredService<IServicioNoRegistrado>();

            // Assert
            action.Should()
                .Throw<InvalidOperationException>();
        }
    }

    private interface IServicioNoRegistrado {
    }
}