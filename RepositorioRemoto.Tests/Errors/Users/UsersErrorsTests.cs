using FluentAssertions;
using RepositorioRemoto.Back.Errors.Users;

namespace RepositorioRemoto.Tests.Errors.Users;

[TestFixture]
public class UsersErrorsTests {
    [Test]
    public void NotFoundError_DeberiaAsignarPropiedades_Correctamente() {
        var resource = "User";
        var id = 42;

        var error = new UsersErrors.NotFoundError(resource, id);

        error.Resource.Should().Be(resource);
        error.Id.Should().Be(id);
        error.Should().BeAssignableTo<UsersErrors>();
    }
    
    [Test]
    public void ValidationError_DeberiaAsignarPropiedades_Correctamente() {
        var field = "Email";
        var message = "El formato del email no es válido.";

        var error = new UsersErrors.ValidationError(field, message);

        error.Field.Should().Be(field);
        error.Message.Should().Be(message);
        error.Should().BeAssignableTo<UsersErrors>();
    }
}