using FluentAssertions;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Tests.Models;

[TestFixture]
public class UserTests
{
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente()
    {
        var id = 1;
        var name = "John Doe";
        var userName = "johndoe";
        var email = "john.doe@example.com";
        var address = new Address("Main St", "Apt 1", "City", "12345", new Geo("10.0", "20.0"));
        var phone = "123-456-789";
        var website = "example.com";
        var company = new Company("Acme", "Catchy", "Bs");
        var createAt = DateTime.UtcNow;
        var updateAt = DateTime.UtcNow;
        var deleteAt = DateTime.MinValue;
        var isDeleted = false;

        var user = new User(
            id,
            name,
            userName,
            email,
            address,
            phone,
            website,
            company,
            createAt,
            updateAt,
            deleteAt,
            isDeleted
        );

        user.Id.Should().Be(id);
        user.Name.Should().Be(name);
        user.UserName.Should().Be(userName);
        user.Email.Should().Be(email);
        user.Address.Should().Be(address);
        user.Phone.Should().Be(phone);
        user.Website.Should().Be(website);
        user.Company.Should().Be(company);
        user.CreateAt.Should().Be(createAt);
        user.UpdateAt.Should().Be(updateAt);
        user.DeleteAt.Should().Be(deleteAt);
        user.IsDeleted.Should().Be(isDeleted);
    }
}