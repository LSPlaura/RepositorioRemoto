using RepositorioRemoto.Back.Dto.Users;

namespace RepositorioRemoto.Back.Dto;

public abstract record RequestDto(
    string Name,
    string UserName,
    string Email,
    AddressDto Address,
    string Phone,
    string Website,
    CompanyDto Company);
