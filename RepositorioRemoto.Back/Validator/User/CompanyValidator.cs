using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Users;
using Serilog;

namespace RepositorioRemoto.Back.Validator.User;

/// <summary>
/// Validador para <see cref="CompanyDto"/>
/// como método de extensión para que varios validadores lo puedan utilizar.
/// </summary>
public static class CompanyValidator {
    private static readonly ILogger _logger = Log.ForContext(typeof(CompanyValidator));

    private static readonly Regex _regexName = new(@"^[A-Za-z0-9\s.,#-]{2,100}$");
    private static readonly Regex _regexCatchPhrase = new(@"^[A-Za-z0-9\s.,#-]{3,150}$");
    private static readonly Regex _regexBs = new(@"^[A-Za-z0-9\s.,#-]{3,150}$");

    public static Result<bool, DomainError> CheckEmptyOrWhiteSpace(this CompanyDto item) {
        if (string.IsNullOrWhiteSpace(item.Name)) {
            _logger.Debug("Error de validación: El campo Name de la empresa está vacío o contiene solo espacios.");
            return UsersErrors.ValidationError("Name", "El nombre de la empresa no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(item.CatchPhrase)) {
            _logger.Debug("Error de validación: El campo CatchPhrase de la empresa está vacío o contiene solo espacios.");
            return UsersErrors.ValidationError("CatchPhrase", "El eslogan de la empresa no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(item.Bs)) {
            _logger.Debug("Error de validación: El campo Bs de la empresa está vacío o contiene solo espacios.");
            return UsersErrors.ValidationError("Bs", "La actividad o sector de la empresa no puede estar vacío.");
        }

        return true;
    }

    public static Result<bool, DomainError> CheckRegex(this CompanyDto item)
    {
        if (!_regexName.IsMatch(item.Name)) {
            _logger.Debug("Error de validación: El formato del campo Name de la empresa no es válido.");
            return UsersErrors.ValidationError("Name", "El formato del nombre de la empresa no es válido.");
        }

        if (!_regexCatchPhrase.IsMatch(item.CatchPhrase)) {
            _logger.Debug("Error de validación: El formato del campo CatchPhrase de la empresa no es válido.");
            return UsersErrors.ValidationError("CatchPhrase", "El formato del eslogan de la empresa no es válido.");
        }

        if (!_regexBs.IsMatch(item.Bs)) {
            _logger.Debug("Error de validación: El formato del campo Bs de la empresa no es válido.");
            return UsersErrors.ValidationError("Bs", "El formato de la actividad de la empresa no es válido.");
        }

        return true;
    }
}