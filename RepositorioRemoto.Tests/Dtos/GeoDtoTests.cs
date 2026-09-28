using FluentAssertions;
using RepositorioRemoto.Back.Dtos;

namespace RepositorioRemoto.Tests.Dtos;

[TestFixture]
public class GeoDtoTests
{
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente()
    {
        var lat = "-37.3159";
        var lng = "81.1496";

        var geoDto = new GeoDto(lat, lng);

        geoDto.Lat.Should().Be(lat);
        geoDto.Lng.Should().Be(lng);
    }
}