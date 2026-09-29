using FluentAssertions;
using RepositorioRemoto.Back.Dto;

namespace RepositorioRemoto.Tests.Dto;

[TestFixture]
public class AddressDtoTests {
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente() {
        var street = "Calle Falsa 123";
        var suite = "Apto 4B";
        var city = "Madrid";
        var zipCode = "28001";
        var geo = new GeoDto("40.4167", "-3.7037");

        var addressDto = new AddressDto(street, suite, city, zipCode, geo);

        addressDto.Street.Should().Be(street);
        addressDto.Suite.Should().Be(suite);
        addressDto.City.Should().Be(city);
        addressDto.ZipCode.Should().Be(zipCode);
        addressDto.Geo.Should().Be(geo);
    }
}