using FluentAssertions;
using RepositorioRemoto.Back.Mappers;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Tests.Mappers;

[TestFixture]
public class CompanyMapperTests {

    [TestFixture]
    public sealed class CasosValidos {

        private Company _company = null!;

        [SetUp]
        public void Setup() {
            _company = new Company(
                Name: "Romaguera-Crona",
                CatchPhrase: "Multi-layered client-server neural-net",
                Bs: "harness real-time e-markets"
            );
        }

        [Test]
        public void ToJson_CompanyValida_ConvierteCorrectamente() {
            // Arrange

            // Act
            var res = _company.ToJson();

            // Assert
            res.Should().NotBeNullOrEmpty();
            res.Should().Contain("\"name\":\"Romaguera-Crona\"");
            res.Should().Contain(
                "\"catchPhrase\":\"Multi-layered client-server neural-net\""
            );
            res.Should().Contain(
                "\"bs\":\"harness real-time e-markets\""
            );
        }

        [Test]
        public void ToCompany_JsonValido_ConvierteCorrectamente() {
            // Arrange
            const string json =
                """
                {
                    "name": "Romaguera-Crona",
                    "catchPhrase": "Multi-layered client-server neural-net",
                    "bs": "harness real-time e-markets"
                }
                """;

            // Act
            var res = json.ToCompany();

            // Assert
            res.Should().NotBeNull();
            res.Name.Should().Be("Romaguera-Crona");
            res.CatchPhrase.Should()
                .Be("Multi-layered client-server neural-net");
            res.Bs.Should().Be("harness real-time e-markets");
        }

        [Test]
        public void ToJsonYToCompany_CompanyValida_MantieneDatos() {
            // Arrange

            // Act
            var json = _company.ToJson();
            var res = json.ToCompany();

            // Assert
            res.Should().NotBeNull();
            res.Name.Should().Be(_company.Name);
            res.CatchPhrase.Should().Be(_company.CatchPhrase);
            res.Bs.Should().Be(_company.Bs);
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        [Test]
        public void ToJson_CompanyNula_RetornaCadenaVacia() {
            // Arrange
            Company? company = null;

            // Act
            var res = company.ToJson();

            // Assert
            res.Should().NotBeNull();
            res.Should().BeEmpty();
        }

        [Test]
        public void ToCompany_JsonNulo_RetornaCompanyPorDefecto() {
            // Arrange
            string? json = null;

            // Act
            var res = json.ToCompany();

            // Assert
            res.Should().NotBeNull();
            res.Name.Should().BeEmpty();
            res.CatchPhrase.Should().BeEmpty();
            res.Bs.Should().BeEmpty();
        }

        [Test]
        public void ToCompany_JsonVacio_RetornaCompanyPorDefecto() {
            // Arrange
            const string json = "";

            // Act
            var res = json.ToCompany();

            // Assert
            res.Should().NotBeNull();
            res.Name.Should().BeEmpty();
            res.CatchPhrase.Should().BeEmpty();
            res.Bs.Should().BeEmpty();
        }

        [Test]
        public void ToCompany_JsonConEspacios_RetornaCompanyPorDefecto() {
            // Arrange
            const string json = "   ";

            // Act
            var res = json.ToCompany();

            // Assert
            res.Should().NotBeNull();
            res.Name.Should().BeEmpty();
            res.CatchPhrase.Should().BeEmpty();
            res.Bs.Should().BeEmpty();
        }

        [Test]
        public void ToCompany_JsonInvalido_RetornaCompanyPorDefecto() {
            // Arrange
            const string json = "{ esto no es un json valido }";

            // Act
            var res = json.ToCompany();

            // Assert
            res.Should().NotBeNull();
            res.Name.Should().BeEmpty();
            res.CatchPhrase.Should().BeEmpty();
            res.Bs.Should().BeEmpty();
        }
    }
}