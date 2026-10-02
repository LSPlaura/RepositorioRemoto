namespace RepositorioRemoto.Back.Models;

public record Geo
{
    public string Lat { get; init; } = null!;
    public string Lng { get; init; } = null!;

    protected Geo() { }

    public Geo(string Lat, string Lng)
    {
        this.Lat = Lat;
        this.Lng = Lng;
    }
}