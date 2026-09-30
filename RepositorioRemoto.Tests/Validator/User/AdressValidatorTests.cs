using FluentAssertions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Validator.User;

namespace RepositorioRemoto.Tests.Validator.User;

[TestFixture]
public class AddressValidatorTests
{
    private static GeoDto CrearGeoValido() => new("-40.4167754", "-3.7037902");

    [TestFixture]
    public class CasosValidos
    {
        [Test]
        public void CheckEmptyOrWhiteSpace_DireccionConCamposValidos_DebeSerExitoso()
        {
            // Arrange
            var address = new AddressDto(
                "Calle Mayor 123",
                "Piso 2 B",
                "Madrid",
                "28001",
                CrearGeoValido()
            );

            // Act
            var result = address.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
        }
        
        [TestCase("123")] // Min exacto: 3 chars
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Max exacto: 100 chars
        [TestCase("Calle 123, Av.#-")] // Caracteres especiales permitidos (.,#-)
        public void CheckRegex_StreetEnValoresLimiteValidos_DebeSerExitoso(string street)
        {
            // Arrange
            var address = new AddressDto(street, "Apt 1", "Madrid", "28001", CrearGeoValido());

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
        
        [TestCase("A")] // Min exacto: 1 char
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Max exacto: 50 chars
        [TestCase("Piso 3, .#-")] // Caracteres especiales permitidos
        public void CheckRegex_SuiteEnValoresLimiteValidos_DebeSerExitoso(string suite)
        {
            // Arrange
            var address = new AddressDto("Calle Mayor", suite, "Madrid", "28001", CrearGeoValido());

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
        
        [TestCase("NY")] // Min exacto: 2 chars
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Max exacto: 50 chars
        [TestCase("Ciudad 12, .#-")] // Caracteres especiales permitidos
        public void CheckRegex_CityEnValoresLimiteValidos_DebeSerExitoso(string city)
        {
            // Arrange
            var address = new AddressDto("Calle Mayor", "Apt 1", city, "28001", CrearGeoValido());

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
        
        [TestCase("123")] // Min exacto: 3 chars
        [TestCase("1234567890")] // Max exacto: 10 chars
        [TestCase("28001-AB")] // Caracteres especiales permitidos (\s-)
        public void CheckRegex_ZipCodeEnValoresLimiteValidos_DebeSerExitoso(string zipCode)
        {
            // Arrange
            var address = new AddressDto("Calle Mayor", "Apt 1", "Madrid", zipCode, CrearGeoValido());

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
        
        [TestCase("0", "0")] // Valores mínimos
        [TestCase("999", "-999")] // Valores extremos enteras (3 dígitos)
        [TestCase("40.1", "-3.12345678")] // Con hasta 8 decimales
        [TestCase("-999.99999999", "999.99999999")] // Límites máximo dígitos y decimales
        public void CheckRegex_GeoCoordenadasValidas_DebeSerExitoso(string lat, string lng)
        {
            // Arrange
            var geo = new GeoDto(lat, lng);
            var address = new AddressDto("Calle Mayor", "Apt 1", "Madrid", "28001", geo);

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosInvalidos
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void CheckEmptyOrWhiteSpace_StreetVacioOEspacios_DebeRetornarFallo(string? street)
        {
            // Arrange
            var address = new AddressDto(street!, "Apt 1", "Madrid", "28001", CrearGeoValido());

            // Act
            var result = address.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void CheckEmptyOrWhiteSpace_SuiteVacioOEspacios_DebeRetornarFallo(string? suite)
        {
            // Arrange
            var address = new AddressDto("Calle Mayor", suite!, "Madrid", "28001", CrearGeoValido());

            // Act
            var result = address.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void CheckEmptyOrWhiteSpace_CityVacioOEspacios_DebeRetornarFallo(string? city)
        {
            // Arrange
            var address = new AddressDto("Calle Mayor", "Apt 1", city!, "28001", CrearGeoValido());

            // Act
            var result = address.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void CheckEmptyOrWhiteSpace_ZipCodeVacioOEspacios_DebeRetornarFallo(string? zipCode)
        {
            // Arrange
            var address = new AddressDto("Calle Mayor", "Apt 1", "Madrid", zipCode!, CrearGeoValido());

            // Act
            var result = address.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void CheckEmptyOrWhiteSpace_GeoLatVacioOEspacios_DebeRetornarFallo(string? lat)
        {
            // Arrange
            var geo = new GeoDto(lat!, "-3.7037902");
            var address = new AddressDto("Calle Mayor", "Apt 1", "Madrid", "28001", geo);

            // Act
            var result = address.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void CheckEmptyOrWhiteSpace_GeoLngVacioOEspacios_DebeRetornarFallo(string? lng)
        {
            // Arrange
            var geo = new GeoDto("-40.4167754", lng!);
            var address = new AddressDto("Calle Mayor", "Apt 1", "Madrid", "28001", geo);

            // Act
            var result = address.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("Ab")] // Justo debajo del Min (2 chars)
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Justo sobre el Max (101 chars)
        [TestCase("Calle@Mayor")] // Caracter no permitido '@'
        [TestCase("Calle!")] // Caracter no permitido '!'
        [TestCase("Calle$")] // Caracter no permitido '$'
        [TestCase("Calle_")] // Caracter no permitido '_'
        public void CheckRegex_StreetFueraDeLimitesOCaracterNoPermitido_DebeRetornarFallo(string street)
        {
            // Arrange
            var address = new AddressDto(street, "Apt 1", "Madrid", "28001", CrearGeoValido());

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Justo sobre el Max (51 chars)
        [TestCase("Piso@1")] // Caracter no permitido '@'
        [TestCase("Piso!")] // Caracter no permitido '!'
        [TestCase("Piso_")] // Caracter no permitido '_'
        public void CheckRegex_SuiteFueraDeLimitesOCaracterNoPermitido_DebeRetornarFallo(string suite)
        {
            // Arrange
            var address = new AddressDto("Calle Mayor", suite, "Madrid", "28001", CrearGeoValido());

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("A")] // Justo debajo del Min (1 char)
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Justo sobre el Max (51 chars)
        [TestCase("Madrid@")] // Caracter no permitido '@'
        [TestCase("Madrid!")] // Caracter no permitido '!'
        [TestCase("Madrid_")] // Caracter no permitido '_'
        public void CheckRegex_CityFueraDeLimitesOCaracterNoPermitido_DebeRetornarFallo(string city)
        {
            // Arrange
            var address = new AddressDto("Calle Mayor", "Apt 1", city, "28001", CrearGeoValido());

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("12")] // Justo debajo del Min (2 chars)
        [TestCase("12345678901")] // Justo sobre el Max (11 chars)
        [TestCase("2800.1")] // Caracter no permitido '.' en ZipCode
        [TestCase("2800#1")] // Caracter no permitido '#' en ZipCode
        [TestCase("28001@")] // Caracter no permitido '@'
        [TestCase("28001_")] // Caracter no permitido '_'
        public void CheckRegex_ZipCodeFueraDeLimitesOCaracterNoPermitido_DebeRetornarFallo(string zipCode)
        {
            // Arrange
            var address = new AddressDto("Calle Mayor", "Apt 1", "Madrid", zipCode, CrearGeoValido());

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("1000")] // Más de 3 dígitos enteros
        [TestCase("-1000")] // Más de 3 dígitos enteros negativos
        [TestCase("40.123456789")] // Más de 8 decimales
        [TestCase("abc")] // Texto no numérico
        [TestCase("40,4167")] // Coma en lugar de punto decimal
        public void CheckRegex_GeoLatFormatoIncorrecto_DebeRetornarFallo(string lat)
        {
            // Arrange
            var geo = new GeoDto(lat, "-3.7037902");
            var address = new AddressDto("Calle Mayor", "Apt 1", "Madrid", "28001", geo);

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("1000")] // Más de 3 dígitos enteros
        [TestCase("-1000")] // Más de 3 dígitos enteros negativos
        [TestCase("-3.123456789")] // Más de 8 decimales
        [TestCase("xyz")] // Texto no numérico
        [TestCase("-3,7037")] // Coma en lugar de punto decimal
        public void CheckRegex_GeoLngFormatoIncorrecto_DebeRetornarFallo(string lng)
        {
            // Arrange
            var geo = new GeoDto("-40.4167754", lng);
            var address = new AddressDto("Calle Mayor", "Apt 1", "Madrid", "28001", geo);

            // Act
            var result = address.CheckRegex();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}