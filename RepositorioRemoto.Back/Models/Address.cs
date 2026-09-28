namespace RepositorioRemoto.Back.Models;

public record Address(
    string Street,
    string Suite,
    string City,
    string ZipCode,
    Geo Geo
);