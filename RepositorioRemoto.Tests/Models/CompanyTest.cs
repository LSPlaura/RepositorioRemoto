using FluentAssertions;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Tests.Models;

[TestFixture]
public class CompanyTest {
    [Test]
    public void Constructor_DeberiaAsignarPropiedades_Correctamente() {
        var name = "Acme Corp";
        var catchPhrase = "Quality First";
        var bs = "synergize scalable solutions";

        var company = new Company(name, catchPhrase, bs);

        company.Name.Should().Be(name);
        company.CatchPhrase.Should().Be(catchPhrase);
        company.Bs.Should().Be(bs);
    }
}