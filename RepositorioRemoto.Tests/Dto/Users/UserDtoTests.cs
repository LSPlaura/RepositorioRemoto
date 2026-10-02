using FluentAssertions;
using RepositorioRemoto.Back.Dto.Users;

namespace RepositorioRemoto.Tests.Dto.Users;

[TestFixture]
public class UserDtoTests {
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente() {
        var id = 1;
        var name = "Laura";
        var userName = "lauradev";
        var email = "laura@example.com";
        var address = new AddressDto("Calle Principal 1", "Apto 4B", "Madrid", "28001", new GeoDto("40.4167", "-3.7037"));
        var phone = "600000000";
        var website = "laura.dev";
        var company = new CompanyDto("DevCorp", "Innovating", "Solutions");

        var userDto = new UserDto(
            id,
            name,
            userName,
            email,
            address,
            phone,
            website,
            company
        );

        userDto.Id.Should().Be(id);
        userDto.Name.Should().Be(name);
        userDto.UserName.Should().Be(userName);
        userDto.Email.Should().Be(email);
        userDto.Address.Should().Be(address);
        userDto.Phone.Should().Be(phone);
        userDto.Website.Should().Be(website);
        userDto.Company.Should().Be(company);
    }
}