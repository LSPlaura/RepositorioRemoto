using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using RepositorioRemoto.Back.Errors;
using RepositorioRemoto.Back.Errors.Storage;
using RepositorioRemoto.Back.Models;
using Serilog;

namespace RepositorioRemoto.Back.Storage;

public class UserStorage : IUserStorage {
    private readonly ILogger _logger = Log.ForContext<UserStorage>();

    private readonly JsonSerializerOptions _options = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Inicializa el almacenamiento para la exportación de usuarios en formato JSON.
    /// </summary>
    public UserStorage() {
        _logger.Debug("Se está iniciando el almacenamiento de usuarios en JSON.");
    }

    /// <inheritdoc cref="IUserStorage.ExportarJsonAsync" />
    public async Task<Result<bool, DomainError>> ExportarJsonAsync(IEnumerable<User> items, string path) {
        _logger.Debug("Intentando exportar los usuarios al archivo JSON: {Path}", path);

        try {
            InitStorage(path);

            var json = JsonSerializer.Serialize(items.ToList(), _options);

            await File.WriteAllTextAsync(path, json, new UTF8Encoding(false));

            _logger.Information("Usuarios exportados correctamente a {Path}", path);

            return Result.Success<bool, DomainError>(true);
        } catch (Exception e) {
            _logger.Error(e, "Error al exportar los usuarios al archivo JSON: {Path}", path);

            return Result.Failure<bool, DomainError>(
                StorageErrors.WriteError(e.Message)
            );
        }
    }

    /// <summary>
    /// Crea el directorio de destino si no existe.
    /// </summary>
    /// <param name="path">Ruta del archivo JSON que se va a generar.</param>
    private void InitStorage(string path) {
        var directory = Path.GetDirectoryName(path);

        if (string.IsNullOrEmpty(directory) || Directory.Exists(directory)) return;

        _logger.Debug("Creando el directorio de almacenamiento: {Directory}", directory);
        Directory.CreateDirectory(directory);
    }
}