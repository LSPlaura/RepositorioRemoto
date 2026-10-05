using System.Text.Json;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Mappers;

/// <summary>
/// Métodos de extensión para mapeo y conversión de AddressDto.
/// </summary>
public static class AddressMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Instancia por defecto con todas las cadenas vacías.
    /// </summary>
    private static Address DefaultAddress => new(
        Street: string.Empty,
        Suite: string.Empty,
        City: string.Empty,
        ZipCode: string.Empty,
        Geo: new Geo(Lat: string.Empty, Lng: string.Empty)
    );

    /// <summary>
    /// Convierte una instancia de AddressDto a una cadena en formato JSON.
    /// </summary>
    public static string ToJson(this Address? address)
    {
        if (address is null) return string.Empty;
        
        return JsonSerializer.Serialize(address, JsonOptions);
    }

    /// <summary>
    /// Convierte una cadena JSON a AddressDto. Retorna una instancia por defecto con cadenas vacías en caso de error o valor nulo.
    /// </summary>
    public static Address ToAddress(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return DefaultAddress;
        }

        try
        {
            var result = JsonSerializer.Deserialize<Address>(json, JsonOptions);
            return result ?? DefaultAddress;
        }
        catch (JsonException)
        {
            return DefaultAddress;
        }
    }
    
    /// <summary>
    /// Convierte un AddressDto al modelo de dominio Address.
    /// </summary>
    /// <param name="dto">Dirección que se quiere convertir.</param>
    /// <returns>Dirección convertida al modelo de dominio.</returns>
    public static Address ToModel(this AddressDto dto) {
        return new Address(
            Street: dto.Street,
            Suite: dto.Suite,
            City: dto.City,
            ZipCode: dto.ZipCode,
            Geo: new Geo(
                Lat: dto.Geo.Lat,
                Lng: dto.Geo.Lng
            )
        );
    }
}