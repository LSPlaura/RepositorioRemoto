using FluentAssertions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Validator.User;

namespace RepositorioRemoto.Test.Validator.User;

[TestFixture]
public class RequestValidatorTests {

    [TestFixture]
    public sealed class CasosValidos {

        private RequestValidator _validator = null!;

        [SetUp]
        public void SetUp() {
            _validator = new RequestValidator();
        }

        [Test]
        public void Validate_RequestValido_RetornaSuccess() {
            // Arrange
            var request = CrearRequestValido();

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsSuccess.Should().BeTrue();
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        private RequestValidator _validator = null!;

        [SetUp]
        public void SetUp() {
            _validator = new RequestValidator();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Validate_NameVacioONulo_RetornaFailure(string? name) {
            // Arrange
            var request = CrearRequestValido(name: name);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Validate_UserNameVacioONulo_RetornaFailure(string? userName) {
            // Arrange
            var request = CrearRequestValido(userName: userName);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Validate_EmailVacioONulo_RetornaFailure(string? email) {
            // Arrange
            var request = CrearRequestValido(email: email);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_AddressConCampoVacio_RetornaFailure() {
            // Arrange
            var address = CrearAddressValido(street: "");
            var request = CrearRequestValido(address: address);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Validate_PhoneVacioONulo_RetornaFailure(string? phone) {
            // Arrange
            var request = CrearRequestValido(phone: phone);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Validate_WebsiteVacioONulo_RetornaFailure(string? website) {
            // Arrange
            var request = CrearRequestValido(website: website);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_CompanyConCampoVacio_RetornaFailure() {
            // Arrange
            var company = CrearCompanyValida(name: "");
            var request = CrearRequestValido(company: company);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("A")]
        [TestCase("Lucia2")]
        [TestCase("Lucia@")]
        public void Validate_NameFormatoInvalido_RetornaFailure(string name) {
            // Arrange
            var request = CrearRequestValido(name: name);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("ab")]
        [TestCase("usuario@")]
        [TestCase("usuario!")]
        public void Validate_UserNameFormatoInvalido_RetornaFailure(string userName) {
            // Arrange
            var request = CrearRequestValido(userName: userName);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("correo")]
        [TestCase("correo@")]
        [TestCase("correo@correo")]
        public void Validate_EmailFormatoInvalido_RetornaFailure(string email) {
            // Arrange
            var request = CrearRequestValido(email: email);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_AddressRegexInvalida_RetornaFailure() {
            // Arrange
            var address = CrearAddressValido(street: "@@@");
            var request = CrearRequestValido(address: address);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("telefono")]
        [TestCase("@@@")]
        public void Validate_PhoneFormatoInvalido_RetornaFailure(string phone) {
            // Arrange
            var request = CrearRequestValido(phone: phone);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("website")]
        [TestCase("localhost")]
        public void Validate_WebsiteFormatoInvalido_RetornaFailure(string website) {
            // Arrange
            var request = CrearRequestValido(website: website);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_CompanyRegexInvalida_RetornaFailure() {
            // Arrange
            var company = CrearCompanyValida(name: "@@@");
            var request = CrearRequestValido(company: company);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }
    }

    private static CreateUserRequest CrearRequestValido(
        string? name = "Lucia Fuertes",
        string? userName = "lucia_027",
        string? email = "lucia@example.com",
        AddressDto? address = null,
        string? phone = "+34 612 345 678",
        string? website = "https://example.com",
        CompanyDto? company = null
    ) {
        return new CreateUserRequest(
            name!,
            userName!,
            email!,
            address ?? CrearAddressValido(),
            phone!,
            website!,
            company ?? CrearCompanyValida()
        );
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

    private static CompanyDto CrearCompanyValida(
        string? name = "Romaguera-Crona",
        string? catchPhrase = "Multi-layered client-server",
        string? bs = "harness real-time e-markets"
    ) {
        return new CompanyDto(
            name!,
            catchPhrase!,
            bs!
        );
    }
}