using FluentAssertions;
using RepositorioRemoto.Back.Dto;

namespace RepositorioRemoto.Tests.Dto;

[TestFixture]
public class UpdateUserRequestTests {
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente() {
        var id = 1;
        var name = "Lucia";
        var userName = "luluchandev";
        var email = "luluchan@example.com";
        var address = "Calle Principal 1";
        var phone = "600000000";
        var website = "luluchan.dev";
        var company = "DevCorp";

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