using System.Text.Json;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Mappers;

/// <summary>
/// Métodos de extensión para mapeo y conversión de Address, AddressDto, Geo y GeoDto.
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
    /// Convierte una instancia de Address a una cadena en formato JSON.
    /// </summary>
    public static string ToJson(this Address? address)
    {
        if (address is null) return string.Empty;
        
        return JsonSerializer.Serialize(address, JsonOptions);
    }

    /// <summary>
    /// Convierte una cadena JSON a Address. Retorna una instancia por defecto con cadenas vacías en caso de error o valor nulo.
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
    public static Address ToModel(this AddressDto dto)
    {
        return new Address(
            Street: dto.Street,
            Suite: dto.Suite,
            City: dto.City,
            ZipCode: dto.ZipCode,
            Geo: dto.Geo.ToModel()
        );
    }

    /// <summary>
    /// Convierte un GeoDto al modelo de dominio Geo.
    /// </summary>
    /// <param name="dto">GeoDTO que se quiere convertir.</param>
    /// <returns>Geo convertido al modelo de dominio.</returns>
    public static Geo ToModel(this GeoDto dto)
    {
        return new Geo(
            Lat: dto.Lat,
            Lng: dto.Lng
        );
    }

    /// <summary>
    /// Convierte el modelo de dominio Address a AddressDto.
    /// </summary>
    /// <param name="address">Modelo de dominio que se quiere convertir.</param>
    /// <returns>AddressDto resultante.</returns>
    public static AddressDto ToDto(this Address address)
    {
        return new AddressDto(
            Street: address.Street,
            Suite: address.Suite,
            City: address.City,
            ZipCode: address.ZipCode,
            Geo: address.Geo.ToDto()
        );
    }

    /// <summary>
    /// Convierte el modelo de dominio Geo a GeoDto.
    /// </summary>
    /// <param name="geo">Modelo de dominio que se quiere convertir.</param>
    /// <returns>GeoDto resultante.</returns>
    public static GeoDto ToDto(this Geo geo)
    {
        return new GeoDto(
            Lat: geo.Lat,
            Lng: geo.Lng
        );
    }
}