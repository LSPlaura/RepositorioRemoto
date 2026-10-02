using FluentAssertions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Validator.User;

namespace RepositorioRemoto.Test.Validator.User;

[TestFixture]
public class AddressValidatorTests {

    [TestFixture]
    public sealed class CasosValidos {

        [Test]
        public void CheckEmptyOrWhiteSpace_AddressValida_RetornaSuccess() {
            // Arrange
            var address = CrearAddressValido();

            // Act
            var res = address.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void CheckRegex_AddressValida_RetornaSuccess() {
            // Arrange
            var address = CrearAddressValido();

            // Act
            var res = address.CheckRegex();

            // Assert
            res.IsSuccess.Should().BeTrue();
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CheckEmptyOrWhiteSpace_StreetVacioONulo_RetornaFailure(string? street) {
            // Arrange
            var address = CrearAddressValido(street: street);

            // Act
            var res = address.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CheckEmptyOrWhiteSpace_SuiteVacioONulo_RetornaFailure(string? suite) {
            // Arrange
            var address = CrearAddressValido(suite: suite);

            // Act
            var res = address.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CheckEmptyOrWhiteSpace_CityVacioONulo_RetornaFailure(string? city) {
            // Arrange
            var address = CrearAddressValido(city: city);

            // Act
            var res = address.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CheckEmptyOrWhiteSpace_ZipCodeVacioONulo_RetornaFailure(string? zipCode) {
            // Arrange
            var address = CrearAddressValido(zipCode: zipCode);

            // Act
            var res = address.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CheckEmptyOrWhiteSpace_LatVacioONulo_RetornaFailure(string? lat) {
            // Arrange
            var address = CrearAddressValido(lat: lat);

            // Act
            var res = address.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CheckEmptyOrWhiteSpace_LngVacioONulo_RetornaFailure(string? lng) {
            // Arrange
            var address = CrearAddressValido(lng: lng);

            // Act
            var res = address.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("@@")]
        [TestCase("!!")]
        public void CheckRegex_StreetInvalido_RetornaFailure(string street) {
            // Arrange
            var address = CrearAddressValido(street: street);

            // Act
            var res = address.CheckRegex();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("@")]
        [TestCase("!")]
        public void CheckRegex_SuiteInvalido_RetornaFailure(string suite) {
            // Arrange
            var address = CrearAddressValido(suite: suite);

            // Act
            var res = address.CheckRegex();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("@")]
        [TestCase("!")]
        public void CheckRegex_CityInvalido_RetornaFailure(string city) {
            // Arrange
            var address = CrearAddressValido(city: city);

            // Act
            var res = address.CheckRegex();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("@@@")]
        [TestCase("!!")]
        public void CheckRegex_ZipCodeInvalido_RetornaFailure(string zipCode) {
            // Arrange
            var address = CrearAddressValido(zipCode: zipCode);

            // Act
            var res = address.CheckRegex();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("abc")]
        [TestCase("12.12.12")]
        public void CheckRegex_LatInvalido_RetornaFailure(string lat) {
            // Arrange
            var address = CrearAddressValido(lat: lat);

            // Act
            var res = address.CheckRegex();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("abc")]
        [TestCase("12.12.12")]
        public void CheckRegex_LngInvalido_RetornaFailure(string lng) {
            // Arrange
            var address = CrearAddressValido(lng: lng);

            // Act
            var res = address.CheckRegex();

            // Assert
            res.IsFailure.Should().BeTrue();
        }
    }

    private static AddressDto CrearAddressValido(
        string? street = "Kulas Light",
        string? suite = "Apt. 556",
        string? city = "Gwenborough",
        string? zipCode = "92998-3874",
        string? lat = "-37.3159",
        string? lng = "81.1496"
    ) {
        return new AddressDto(
            street!,
            suite!,
            city!,
            zipCode!,
            new GeoDto(
                lat!,
                lng!
            )
        );
    }
}