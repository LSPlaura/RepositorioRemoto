using System.Text.Json;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Mappers;

/// <summary>
/// Métodos de extensión para mapeo y conversión de Company.
/// </summary>
public static class CompanyMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Instancia por defecto con todas las cadenas vacías.
    /// </summary>
    private static Company DefaultCompany => new(
        Name: string.Empty,
        CatchPhrase: string.Empty,
        Bs: string.Empty
    );

    /// <summary>
    /// Convierte una instancia de Company a una cadena en formato JSON.
    /// </summary>
    public static string ToJson(this Company? company)
    {
        if (company is null) return string.Empty;
        
        return JsonSerializer.Serialize(company, JsonOptions);
    }

    /// <summary>
    /// Convierte una cadena JSON a Company. Retorna una instancia por defecto con cadenas vacías en caso de error o valor nulo.
    /// </summary>
    public static Company ToCompany(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return DefaultCompany;
        }

        try
        {
            var result = JsonSerializer.Deserialize<Company>(json, JsonOptions);
            return result ?? DefaultCompany;
        }
        catch (JsonException)
        {
            return DefaultCompany;
        }
    }
}