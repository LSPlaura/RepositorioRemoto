using FluentAssertions;
using RepositorioRemoto.Back.Dto;
using RepositorioRemoto.Back.Dtos;

namespace RepositorioRemoto.Tests.Models;

[TestFixture]
public class AdressTests
{
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente()
    {
        var street = "Calle Falsa";
        var suite = "Apt 123";
        var city = "Springfield";
        var zipCode = "12345-6789";
        var geo = new GeoDto("-37.3159", "81.1496"); 
        
        var result = new AddressDto(street, suite, city, zipCode, geo);
        
        result.Street.Should().Be(street);
        result.Suite.Should().Be(suite);
        result.City.Should().Be(city);
        result.ZipCode.Should().Be(zipCode);
        result.Geo.Should().Be(geo);
    }
}