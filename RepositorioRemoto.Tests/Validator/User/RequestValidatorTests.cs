using FluentAssertions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Validator.User;

namespace RepositorioRemoto.Tests.Validator.User;

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
            var request = CrearUserValido();

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
            var request = CrearUserValido(name: name);

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
            var request = CrearUserValido(userName: userName);

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
            var request = CrearUserValido(email: email);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_AddressConCampoVacio_RetornaFailure() {
            // Arrange
            var address = CrearAddressValido(street: "");
            var request = CrearUserValido(address: address);

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
            var request = CrearUserValido(phone: phone);

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
            var request = CrearUserValido(website: website);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_CompanyConCampoVacio_RetornaFailure() {
            // Arrange
            var company = CrearCompanyValida(name: "");
            var request = CrearUserValido(company: company);

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
            var request = CrearUserValido(name: name);

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
            var request = CrearUserValido(userName: userName);

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
            var request = CrearUserValido(email: email);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_AddressRegexInvalida_RetornaFailure() {
            // Arrange
            var address = CrearAddressValido(street: "@@@");
            var request = CrearUserValido(address: address);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("telefono")]
        [TestCase("@@@")]
        public void Validate_PhoneFormatoInvalido_RetornaFailure(string phone) {
            // Arrange
            var request = CrearUserValido(phone: phone);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("website")]
        [TestCase("localhost")]
        public void Validate_WebsiteFormatoInvalido_RetornaFailure(string website) {
            // Arrange
            var request = CrearUserValido(website: website);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_CompanyRegexInvalida_RetornaFailure() {
            // Arrange
            var company = CrearCompanyValida(name: "@@@");
            var request = CrearUserValido(company: company);

            // Act
            var res = _validator.Validate(request);

            // Assert
            res.IsFailure.Should().BeTrue();
        }
    }

    private static Back.Models.User CrearUserValido(
        int id = 23,
        string name = "Lucia Fuertes",
        string userName = "lucia_027",
        string email = "lucia@example.com",
        Address? address = null,
        string phone = "+34 612 345 678",
        string website = "https://example.com",
        Company? company = null,
        DateTime createdAt = default,
        DateTime updatedAt = default,
        DateTime deletedAt = default, 
        bool isActive = true  
    ) {
        return new Back.Models.User(
            id,
            name,
            userName,
            email,
            address ?? CrearAddressValido(),
            phone,
            website,
            company ?? CrearCompanyValida(),
            createdAt,
            updatedAt,
            deletedAt,
            isActive
        );
    }

    private static Address CrearAddressValido(
        string? street = "Kulas Light",
        string? suite = "Apt. 556",
        string? city = "Gwenborough",
        string? zipCode = "92998-3874",
        string? lat = "-37.3159",
        string? lng = "81.1496"
    ) {
        return new Address(
            street!,
            suite!,
            city!,
            zipCode!,
            new Geo(
                lat!,
                lng!
            )
        );
    }

    private static Company CrearCompanyValida(
        string? name = "Romaguera-Crona",
        string? catchPhrase = "Multi-layered client-server",
        string? bs = "harness real-time e-markets"
    ) {
        return new Company(
            name!,
            catchPhrase!,
            bs!
        );
    }
}