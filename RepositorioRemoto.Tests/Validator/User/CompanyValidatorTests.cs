using FluentAssertions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Validator.User;

namespace RepositorioRemoto.Tests.Validator.User;

[TestFixture]
public class CompanyValidatorTests {

    [TestFixture]
    public sealed class CasosValidos {

        [Test]
        public void CheckEmptyOrWhiteSpace_CompanyValida_RetornaSuccess() {
            // Arrange
            var company = CrearCompanyValida();

            // Act
            var res = company.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void CheckRegex_CompanyValida_RetornaSuccess() {
            // Arrange
            var company = CrearCompanyValida();

            // Act
            var res = company.CheckRegex();

            // Assert
            res.IsSuccess.Should().BeTrue();
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CheckEmptyOrWhiteSpace_NameVacioONulo_RetornaFailure(string? name) {
            // Arrange
            var company = CrearCompanyValida(name: name);

            // Act
            var res = company.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CheckEmptyOrWhiteSpace_CatchPhraseVacioONulo_RetornaFailure(string? catchPhrase) {
            // Arrange
            var company = CrearCompanyValida(catchPhrase: catchPhrase);

            // Act
            var res = company.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CheckEmptyOrWhiteSpace_BsVacioONulo_RetornaFailure(string? bs) {
            // Arrange
            var company = CrearCompanyValida(bs: bs);

            // Act
            var res = company.CheckEmptyOrWhiteSpace();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("@")]
        [TestCase("!")]
        public void CheckRegex_NameInvalido_RetornaFailure(string name) {
            // Arrange
            var company = CrearCompanyValida(name: name);

            // Act
            var res = company.CheckRegex();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("@@")]
        [TestCase("!!")]
        public void CheckRegex_CatchPhraseInvalido_RetornaFailure(string catchPhrase) {
            // Arrange
            var company = CrearCompanyValida(catchPhrase: catchPhrase);

            // Act
            var res = company.CheckRegex();

            // Assert
            res.IsFailure.Should().BeTrue();
        }

        [TestCase("@@")]
        [TestCase("!!")]
        public void CheckRegex_BsInvalido_RetornaFailure(string bs) {
            // Arrange
            var company = CrearCompanyValida(bs: bs);

            // Act
            var res = company.CheckRegex();

            // Assert
            res.IsFailure.Should().BeTrue();
        }
    }

    private static Company CrearCompanyValida(string? name = "Romaguera-Crona",
        string? catchPhrase = "Multi-layered client-server",
        string? bs = "harness real-time e-markets") {
        return new Company(
            name!,
            catchPhrase!,
            bs!
        );
    }
}