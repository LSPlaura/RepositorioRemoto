using FluentAssertions;
using RepositorioRemoto.Back.Dto;
using RepositorioRemoto.Back.Dto.Users;

namespace RepositorioRemoto.Tests.Dto;

[TestFixture]
public class CompanyDtoTets {
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente() {
        var name = "Tech Solutions";
        var catchPhrase = "Innovating the future";
        var bs = "e-commerce solutions";

        var companyDto = new CompanyDto(name, catchPhrase, bs);

        companyDto.Name.Should().Be(name);
        companyDto.CatchPhrase.Should().Be(catchPhrase);
        companyDto.Bs.Should().Be(bs);
    }
}