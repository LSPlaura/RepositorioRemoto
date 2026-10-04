namespace RepositorioRemoto.Back.Models;

/// <summary>
/// Representa la geolocalizacion de un usuario.
/// </summary>
public record Geo(
    string Lat,
    string Lng
) {
    public Geo() : this("", "") { }
}