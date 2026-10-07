using System.Text.Json;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Back.Mappers;

/// <summary>
/// Métodos de extensión para mapeo y conversión de Company y CompanyDto.
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
    
    /// <summary>
    /// Convierte un CompanyDto al modelo de dominio Company.
    /// </summary>
    /// <param name="dto">Empresa que se quiere convertir.</param>
    /// <returns>Empresa convertida al modelo de dominio.</returns>
    public static Company ToModel(this CompanyDto dto)
    {
        return new Company(
            Name: dto.Name,
            CatchPhrase: dto.CatchPhrase,
            Bs: dto.Bs
        );
    }

    /// <summary>
    /// Convierte el modelo de dominio Company a CompanyDto.
    /// </summary>
    /// <param name="company">Empresa del modelo de dominio que se quiere convertir.</param>
    /// <returns>CompanyDto resultante.</returns>
    public static CompanyDto ToDto(this Company company)
    {
        return new CompanyDto(
            Name: company.Name,
            CatchPhrase: company.CatchPhrase,
            Bs: company.Bs
        );
    }
}