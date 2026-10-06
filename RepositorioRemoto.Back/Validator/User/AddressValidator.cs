using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Users;
using RepositorioRemoto.Back.Models;
using Serilog;

namespace RepositorioRemoto.Back.Validator.User;

/// <summary>
/// Validador para <see cref="AddressDto"/>
/// como método de extensión para que varios validadores lo puedan utilizar.
/// </summary>
public static class AddressValidator {
    private static readonly ILogger _logger = Log.ForContext(typeof(AddressValidator));

    private static readonly Regex _regexStreet = new(@"^[A-Za-z0-9\s.,#-]{3,100}$");
    private static readonly Regex _regexSuite = new(@"^[A-Za-z0-9\s.,#-]{1,50}$");
    private static readonly Regex _regexCity = new(@"^[A-Za-z0-9\s.,#-]{2,50}$");
    private static readonly Regex _regexZipCode = new(@"^[A-Za-z0-9\s-]{3,10}$");
    private static readonly Regex _regexGeoCoordinate = new(@"^-?\d{1,3}(\.\d{1,8})?$");

    public static Result<bool, DomainError> CheckEmptyOrWhiteSpace(this Address item) {
        if (string.IsNullOrWhiteSpace(item.Street)) {
            _logger.Debug("Error de validación: El campo Street está vacío o contiene solo espacios.");
            return UsersErrors.ValidationError("Street", "La calle no puede estar vacía.");
        }

        if (string.IsNullOrWhiteSpace(item.Suite)) {
            _logger.Debug("Error de validación: El campo Suite está vacío o contiene solo espacios.");
            return UsersErrors.ValidationError("Suite", "El complemento o piso no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(item.City)) {
            _logger.Debug("Error de validación: El campo City está vacío o contiene solo espacios.");
            return UsersErrors.ValidationError("City", "La ciudad no puede estar vacía.");
        }

        if (string.IsNullOrWhiteSpace(item.ZipCode)) {
            _logger.Debug("Error de validación: El campo ZipCode está vacío o contiene solo espacios.");
            return UsersErrors.ValidationError("ZipCode", "El código postal no puede estar vacío.");
        }

        var geoResult = item.Geo.GeoCheckEmptyOrWhiteSpace();

        if (geoResult.IsFailure) {
            _logger.Debug("Error de validación en la verificación de campos vacíos de Geo.");
            return geoResult;
        }

        return true;
    }

    public static Result<bool, DomainError> CheckRegex(this Address item) {
        if (!_regexStreet.IsMatch(item.Street)) {
            _logger.Debug("Error de validación: El formato del campo Street no es válido.");
            return UsersErrors.ValidationError("Street", "El formato de la calle no es válido.");
        }

        if (!_regexSuite.IsMatch(item.Suite)) {
            _logger.Debug("Error de validación: El formato del campo Suite no es válido.");
            return UsersErrors.ValidationError("Suite", "El formato del piso/complemento no es válido.");
        }

        if (!_regexCity.IsMatch(item.City)) {
            _logger.Debug("Error de validación: El formato del campo City no es válido.");
            return UsersErrors.ValidationError("City", "El formato de la ciudad no es válido.");
        }

        if (!_regexZipCode.IsMatch(item.ZipCode)) {
            _logger.Debug("Error de validación: El formato del campo ZipCode no es válido.");
            return UsersErrors.ValidationError("ZipCode", "El formato del código postal no es válido.");
        }

        var geoRegexResult = item.Geo.GeoCheckRegex();

        if (geoRegexResult.IsFailure) {
            _logger.Debug("Error de validación en las reglas de Regex de Geo.");
            return geoRegexResult;
        }

        return true;
    }

    private static Result<bool, DomainError> GeoCheckEmptyOrWhiteSpace(this Geo item) {
        if (string.IsNullOrWhiteSpace(item.Lat)) {
            _logger.Debug("Error de validación: El campo Lat está vacío o contiene solo espacios.");
            return UsersErrors.ValidationError("Lat", "La latitud no puede estar vacía.");
        }

        if (string.IsNullOrWhiteSpace(item.Lng)) {
            _logger.Debug("Error de validación: El campo Lng está vacío o contiene solo espacios.");
            return UsersErrors.ValidationError("Lng", "La longitud no puede estar vacía.");
        }

        return true;
    }

    private static Result<bool, DomainError> GeoCheckRegex(this Geo item) {
        if (!_regexGeoCoordinate.IsMatch(item.Lat)) {
            _logger.Debug("Error de validación: El formato de la latitud (Lat) no es válido.");
            return UsersErrors.ValidationError("Lat", "El formato de la latitud no es válido.");
        }

        if (!_regexGeoCoordinate.IsMatch(item.Lng)) {
            _logger.Debug("Error de validación: El formato de la longitud (Lng) no es válido.");
            return UsersErrors.ValidationError("Lng", "El formato de la longitud no es válido.");
        }

        return true;
    }
}