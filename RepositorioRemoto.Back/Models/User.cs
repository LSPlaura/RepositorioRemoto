namespace RepositorioRemoto.Back.Models;
public record User(
    int Id,
    string Name,
    string UserName,
    string Email,
    Address Address,
    string Phone,
    string Website,
    Company Company
);
