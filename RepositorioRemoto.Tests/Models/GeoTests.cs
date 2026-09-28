using FluentAssertions;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Tests.Models;

[TestFixture]
public class GeoTests
{
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente()
    {
        var lat = "-37.3159";
        var lng = "81.1496";

        var geoDto = new Geo(lat, lng);

        geoDto.Lat.Should().Be(lat);
        geoDto.Lng.Should().Be(lng);
    }
}