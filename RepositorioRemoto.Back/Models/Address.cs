namespace RepositorioRemoto.Back.Models;

public record Address
{
    public string Street { get; init; } = null!;
    public string Suite { get; init; } = null!;
    public string City { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    public Geo Geo { get; init; } = null!;

    // Constructor sin parámetros para EF Core / PostgreSQL JSON
    protected Address() { }

    // Constructor público con parámetros en MAYÚSCULAS para satisfacer a AddressMapper.cs y al resto del proyecto
    public Address(string Street, string Suite, string City, string ZipCode, Geo Geo)
    {
        this.Street = Street;
        this.Suite = Suite;
        this.City = City;
        this.ZipCode = ZipCode;
        this.Geo = Geo;
    }
}