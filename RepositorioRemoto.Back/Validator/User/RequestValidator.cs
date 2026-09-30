using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Dto;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Users;
using Serilog;

namespace RepositorioRemoto.Back.Validator.User;

/// <summary>
/// Validador para <see cref="RequestDto"/>
/// </summary>
public class RequestValidator : IValidate<RequestDto>
{
    private readonly Serilog.ILogger _logger = Log.ForContext<RequestValidator>();

    private static readonly Regex _regexName = new(@"^[A-Za-z\s.-]{2,50}$");
    private static readonly Regex _regexUserName = new(@"^[A-Za-z0-9._-]{3,30}$");
    private static readonly Regex _regexEmail = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    private static readonly Regex _regexPhone = new(@"^(\+[1-9]\d{0,2}[\s\.-]?)?(\(?\d+\)?[\s\.-]?){2,8}(\s?(x|ext|extension)\s?\d{1,8})?$");
    private static readonly Regex _regexWebsite = new(@"^(https?:\/\/)?(www\.)?[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(\/.*)?$");

    /// <inheritdoc />
    public Result<bool, DomainError> Validate(RequestDto item)
    {
        var emptyCheckResult = CheckEmptyOrWhiteSpace(item);
        if (emptyCheckResult.IsFailure)
            return emptyCheckResult;

        var regexCheckResult = CheckRegex(item);
        if (regexCheckResult.IsFailure)
            return regexCheckResult;

        return true;
    }

    private Result<bool, DomainError> CheckEmptyOrWhiteSpace(RequestDto item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            _logger.Warning("Error de validación: El campo Name está vacío o contiene solo espacios.");
            return new UsersErrors.ValidationError("Name", "El nombre no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(item.UserName))
        {
            _logger.Warning("Error de validación: El campo UserName está vacío o contiene solo espacios.");
            return new UsersErrors.ValidationError("UserName", "El nombre de usuario no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(item.Email))
        {
            _logger.Warning("Error de validación: El campo Email está vacío o contiene solo espacios.");
            return new UsersErrors.ValidationError("Email", "El correo electrónico no puede estar vacío.");
        }

        var addressResult = item.Address.CheckEmptyOrWhiteSpace();
        if (addressResult.IsFailure)
        {
            _logger.Warning("Error de validación en la verificación de campos vacíos de Address.");
            return addressResult;
        }

        if (string.IsNullOrWhiteSpace(item.Phone))
        {
            _logger.Warning("Error de validación: El campo Phone está vacío o contiene solo espacios.");
            return new UsersErrors.ValidationError("Phone", "El teléfono no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(item.Website))
        {
            _logger.Warning("Error de validación: El campo Website está vacío o contiene solo espacios.");
            return new UsersErrors.ValidationError("Website", "El sitio web no puede estar vacío.");
        }

        var companyResult = item.Company.CheckEmptyOrWhiteSpace();
        if (companyResult.IsFailure)
        {
            _logger.Warning("Error de validación en la verificación de campos vacíos de Company.");
            return companyResult;
        }

        return true;
    }

    private Result<bool, DomainError> CheckRegex(RequestDto item)
    {
        if (!_regexName.IsMatch(item.Name))
        {
            _logger.Warning("Error de validación: El formato del campo Name no es válido.");
            return new UsersErrors.ValidationError("Name", "El formato del nombre no es válido.");
        }

        if (!_regexUserName.IsMatch(item.UserName))
        {
            _logger.Warning("Error de validación: El formato del campo UserName no es válido.");
            return new UsersErrors.ValidationError("UserName", "El formato del nombre de usuario no es válido.");
        }

        if (!_regexEmail.IsMatch(item.Email))
        {
            _logger.Warning("Error de validación: El formato del campo Email no es válido.");
            return new UsersErrors.ValidationError("Email", "El formato del correo electrónico no es válido.");
        }

        var addressRegexResult = item.Address.CheckRegex();
        if (addressRegexResult.IsFailure)
        {
            _logger.Warning("Error de validación en las reglas de Regex de Address.");
            return addressRegexResult;
        }

        if (!_regexPhone.IsMatch(item.Phone))
        {
            _logger.Warning("Error de validación: El formato del campo Phone no es válido.");
            return new UsersErrors.ValidationError("Phone", "El formato del teléfono no es válido.");
        }

        if (!_regexWebsite.IsMatch(item.Website))
        {
            _logger.Warning("Error de validación: El formato del campo Website no es válido.");
            return new UsersErrors.ValidationError("Website", "El formato del sitio web no es válido.");
        }

        var companyRegexResult = item.Company.CheckRegex();
        if (companyRegexResult.IsFailure)
        {
            _logger.Warning("Error de validación en las reglas de Regex de Company.");
            return companyRegexResult;
        }

        return true;
    }
}