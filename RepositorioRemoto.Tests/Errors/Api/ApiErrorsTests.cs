using FluentAssertions;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Api;

namespace RepositorioRemoto.Tests.Errors.Api;

[TestFixture]
public class ApiErrorsTests {
    [Test]
    public void ApiError_DeberiaAsignarPropiedades_Correctamente() {
        var statusCode = 404;
        var details = "El recurso solicitado no fue encontrado en el servidor.";

        var error = new ApiError(statusCode, details);

        error.StatusCode.Should().Be(statusCode);
        error.Details.Should().Be(details);
        error.Should().BeAssignableTo<DomainError>();
    }
}