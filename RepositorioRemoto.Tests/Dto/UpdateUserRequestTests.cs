using FluentAssertions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Dto.Users.Request;

namespace RepositorioRemoto.Tests.Dto;

[TestFixture]
public class UpdateUserRequestTests
{
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente()
    {
        var id = 1;
        var name = "Lucia";
        var userName = "luluchandev";
        var email = "luluchan@example.com";
        var address = new AddressDto(
            "Calle Principal 1",
            "Piso 2 B",
            "Madrid",
            "28001",
            new GeoDto("-40.4167754", "-3.7037902")
        );
        var phone = "600000000";
        var website = "luluchan.dev";
        var company = new CompanyDto(
            "DevCorp",
            "Innovacion constante",
            "Soluciones tecnologicas"
        );

        var request = new UpdateUserRequest(
            id,
            name,
            userName,
            email,
            address,
            phone,
            website,
            company
        );

        request.Id.Should().Be(id);
        request.Name.Should().Be(name);
        request.UserName.Should().Be(userName);
        request.Email.Should().Be(email);
        request.Address.Should().Be(address);
        request.Phone.Should().Be(phone);
        request.Website.Should().Be(website);
        request.Company.Should().Be(company);
    }
}