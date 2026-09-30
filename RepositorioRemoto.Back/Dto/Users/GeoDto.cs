namespace RepositorioRemoto.Back.Dto.Users;

/// <summary>
///  Representa el dto de la geolocalizacion dentro del sistema.
/// </summary>
public record GeoDto(
    string Lat,
    string Lng
);