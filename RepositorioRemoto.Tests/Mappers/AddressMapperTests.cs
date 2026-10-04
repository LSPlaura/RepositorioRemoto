using FluentAssertions;
using RepositorioRemoto.Back.Mappers;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Tests.Mappers;

[TestFixture]
public class AddressMapperTests {

    [TestFixture]
    public sealed class CasosValidos {

        private Address _address = null!;

        [SetUp]
        public void Setup() {
            _address = new Address(
                Street: "Kulas Light",
                Suite: "Apt. 556",
                City: "Gwenborough",
                ZipCode: "92998-3874",
                Geo: new Geo(
                    Lat: "-37.3159",
                    Lng: "81.1496"
                )
            );
        }

        [Test]
        public void ToJson_AddressValido_ConvierteCorrectamente() {
            // Arrange

            // Act
            var res = _address.ToJson();

            // Assert
            res.Should().NotBeNullOrEmpty();
            res.Should().Contain("\"street\":\"Kulas Light\"");
            res.Should().Contain("\"suite\":\"Apt. 556\"");
            res.Should().Contain("\"city\":\"Gwenborough\"");
            res.Should().Contain("\"zipCode\":\"92998-3874\"");
            res.Should().Contain("\"lat\":\"-37.3159\"");
            res.Should().Contain("\"lng\":\"81.1496\"");
        }

        [Test]
        public void ToAddress_JsonValido_ConvierteCorrectamente() {
            // Arrange
            const string json =
                """
                {
                    "street": "Kulas Light",
                    "suite": "Apt. 556",
                    "city": "Gwenborough",
                    "zipCode": "92998-3874",
                    "geo": {
                        "lat": "-37.3159",
                        "lng": "81.1496"
                    }
                }
                """;

            // Act
            var res = json.ToAddress();

            // Assert
            res.Should().NotBeNull();
            res.Street.Should().Be("Kulas Light");
            res.Suite.Should().Be("Apt. 556");
            res.City.Should().Be("Gwenborough");
            res.ZipCode.Should().Be("92998-3874");

            res.Geo.Should().NotBeNull();
            res.Geo.Lat.Should().Be("-37.3159");
            res.Geo.Lng.Should().Be("81.1496");
        }

        [Test]
        public void ToJsonYToAddress_AddressValido_MantieneDatos() {
            // Arrange

            // Act
            var json = _address.ToJson();
            var res = json.ToAddress();

            // Assert
            res.Should().NotBeNull();
            res.Street.Should().Be(_address.Street);
            res.Suite.Should().Be(_address.Suite);
            res.City.Should().Be(_address.City);
            res.ZipCode.Should().Be(_address.ZipCode);
            res.Geo.Lat.Should().Be(_address.Geo.Lat);
            res.Geo.Lng.Should().Be(_address.Geo.Lng);
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        [Test]
        public void ToJson_AddressNulo_RetornaCadenaVacia() {
            // Arrange
            Address? address = null;

            // Act
            var res = address.ToJson();

            // Assert
            res.Should().NotBeNull();
            res.Should().BeEmpty();
        }

        [Test]
        public void ToAddress_JsonNulo_RetornaAddressPorDefecto() {
            // Arrange
            string? json = null;

            // Act
            var res = json.ToAddress();

            // Assert
            res.Should().NotBeNull();
            res.Street.Should().BeEmpty();
            res.Suite.Should().BeEmpty();
            res.City.Should().BeEmpty();
            res.ZipCode.Should().BeEmpty();
            res.Geo.Should().NotBeNull();
            res.Geo.Lat.Should().BeEmpty();
            res.Geo.Lng.Should().BeEmpty();
        }

        [Test]
        public void ToAddress_JsonVacio_RetornaAddressPorDefecto() {
            // Arrange
            const string json = "";

            // Act
            var res = json.ToAddress();

            // Assert
            res.Should().NotBeNull();
            res.Street.Should().BeEmpty();
            res.Suite.Should().BeEmpty();
            res.City.Should().BeEmpty();
            res.ZipCode.Should().BeEmpty();
            res.Geo.Lat.Should().BeEmpty();
            res.Geo.Lng.Should().BeEmpty();
        }

        [Test]
        public void ToAddress_JsonConEspacios_RetornaAddressPorDefecto() {
            // Arrange
            const string json = "   ";

            // Act
            var res = json.ToAddress();

            // Assert
            res.Should().NotBeNull();
            res.Street.Should().BeEmpty();
            res.Suite.Should().BeEmpty();
            res.City.Should().BeEmpty();
            res.ZipCode.Should().BeEmpty();
            res.Geo.Lat.Should().BeEmpty();
            res.Geo.Lng.Should().BeEmpty();
        }

        [Test]
        public void ToAddress_JsonInvalido_RetornaAddressPorDefecto() {
            // Arrange
            const string json = "{ esto no es un json valido }";

            // Act
            var res = json.ToAddress();

            // Assert
            res.Should().NotBeNull();
            res.Street.Should().BeEmpty();
            res.Suite.Should().BeEmpty();
            res.City.Should().BeEmpty();
            res.ZipCode.Should().BeEmpty();
            res.Geo.Should().NotBeNull();
            res.Geo.Lat.Should().BeEmpty();
            res.Geo.Lng.Should().BeEmpty();
        }
    }
}