namespace RepositorioRemoto.Back.Dto;

/// <summary>
///  Representa el dto de una dirección dentro del sistema.
/// </summary>
public record AddressDto (
    string Street,
    string Suite,
    string City,
    string ZipCode,
    GeoDto Geo
);