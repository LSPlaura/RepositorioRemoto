namespace RepositorioRemoto.Back.Models;

/// <summary>
/// Representa la compañía de usuario dentro del sistema.
/// </summary>
public record Company
{
    public string Name { get; init; } = null!;
    public string CatchPhrase { get; init; } = null!;
    public string Bs { get; init; } = null!;

    // 1. Constructor sin parámetros para EF Core / PostgreSQL JSON
    protected Company() { }

    // 2. Constructor público con parámetros en MAYÚSCULAS para compatibilidad con el Mapper
    public Company(string Name, string CatchPhrase, string Bs)
    {
        this.Name = Name;
        this.CatchPhrase = CatchPhrase;
        this.Bs = Bs;
    }
}