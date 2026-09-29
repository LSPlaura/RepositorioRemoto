using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Dto;
using RepositorioRemoto.Back.Dto.Users.Request;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Users;

namespace RepositorioRemoto.Back.Validator.User;

/// <summary>
/// Validador para <see cref="CreateUserRequest"/>
/// </summary>
public class RequestValidator : IValidate<RequestDto>
{
    /// <inheritdoc />
    public Result<bool, DomainError> Validate(RequestDto item)
    {
        if (CheckEmptyOrWhiteSpace(item).IsFailure)
            return CheckEmptyOrWhiteSpace(item);

        return true;
    }

    private Result<bool, DomainError> CheckEmptyOrWhiteSpace(RequestDto item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
            return new UsersErrors.ValidationError();

        if (string.IsNullOrWhiteSpace(item.UserName))
            return new UsersErrors.ValidationError();

        if (item.Address.CheckEmptyOrWhiteSpace().IsFailure)
            return item.Address.CheckEmptyOrWhiteSpace();
        
        if (string.IsNullOrWhiteSpace(item.Phone))
            return new UsersErrors.ValidationError();
        
        if (string.IsNullOrWhiteSpace(item.Website))
            return new UsersErrors.ValidationError();

        if (item.Company.CheckEmptyOrWhiteSpace().IsFailure)
            return item.Company.CheckEmptyOrWhiteSpace();
        
        return true;
    }
}