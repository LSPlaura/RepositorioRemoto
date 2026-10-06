using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RepositorioRemoto.Back.Config;

namespace RepositorioRemoto.Test.Config;

[TestFixture]
public class ConfiguracionTests {

    public abstract class ConfiguracionTestsBase {

        private IConfiguration _configuracionOriginal = null!;

        private readonly List<IConfigurationRoot> _configuracionesCreadas = new();
        private readonly Dictionary<string, byte[]?> _archivosOriginales = new();

        [SetUp]
        public void SetUp() {
            _configuracionOriginal = Configuracion.Configuration;
            EstablecerConfiguracion(new Dictionary<string, string?>());
        }

        [TearDown]
        public void TearDown() {
            var configuracionActual = Configuracion.Configuration;

            AsignarConfiguracion(_configuracionOriginal);

            try {
                if (!ReferenceEquals(configuracionActual, _configuracionOriginal)) {
                    var creadaEnTests = false;

                    foreach (var configuracion in _configuracionesCreadas) {
                        if (ReferenceEquals(configuracionActual, configuracion)) {
                            creadaEnTests = true;
                            break;
                        }
                    }

                    if (!creadaEnTests) {
                        (configuracionActual as IDisposable)?.Dispose();
                    }
                }

                foreach (var configuracion in _configuracionesCreadas) {
                    (configuracion as IDisposable)?.Dispose();
                }
            } finally {
                _configuracionesCreadas.Clear();

                foreach (var archivo in _archivosOriginales) {
                    if (archivo.Value is not null) {
                        File.WriteAllBytes(archivo.Key, archivo.Value);
                    } else {
                        File.Delete(archivo.Key);
                    }
                }

                _archivosOriginales.Clear();
            }
        }

        protected void EstablecerConfiguracion(Dictionary<string, string?> valores) {
            var configuracion = new ConfigurationBuilder()
                .AddInMemoryCollection(valores)
                .Build();

            _configuracionesCreadas.Add(configuracion);
            AsignarConfiguracion(configuracion);
        }

        protected void CrearArchivoConfiguracion(string entorno, string contenido) {
            var ruta = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"appsettings.{entorno.ToLowerInvariant()}.json"
            );

            if (!_archivosOriginales.ContainsKey(ruta)) {
                _archivosOriginales[ruta] = File.Exists(ruta)
                    ? File.ReadAllBytes(ruta)
                    : null;
            }

            File.WriteAllText(ruta, contenido);
        }

        private static void AsignarConfiguracion(IConfiguration configuracion) {
            var propiedad = typeof(Configuracion).GetProperty(
                nameof(Configuracion.Configuration),
                BindingFlags.Public | BindingFlags.Static
            )!;

            propiedad.SetValue(null, configuracion);
        }
    }

    [TestFixture]
    [NonParallelizable]
    public sealed class CasosValidos : ConfiguracionTestsBase {

        [Test]
        public void Configuration_RetornaConfiguracionCargada() {
            //Act
            var configuracion = Configuracion.Configuration;

            //Assert
            configuracion.Should().NotBeNull();
            configuracion.Should().BeAssignableTo<IConfiguration>();
        }

        [Test]
        public void ApiName_ConValorConfigurado_RetornaValor() {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["ApiSettings:Name"] = "Production"
            });

            //Act
            var nombre = Configuracion.ApiName;

            //Assert
            nombre.Should().Be("Production");
        }

        [Test]
        public void ApiName_SinValorConfigurado_RetornaDevelopment() {
            //Act
            var nombre = Configuracion.ApiName;

            //Assert
            nombre.Should().Be("Development");
        }

        [Test]
        public void BaseUrl_ConValorConfigurado_RetornaValor() {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["ApiSettings:BaseUrl"] = "https://localhost:5001"
            });

            //Act
            var url = Configuracion.BaseUrl;

            //Assert
            url.Should().Be("https://localhost:5001");
        }

        [Test]
        public void BaseUrl_SinValorConfigurado_RetornaValorPorDefecto() {
            //Act
            var url = Configuracion.BaseUrl;

            //Assert
            url.Should().Be("https://jsonplaceholder.typicode.com");
        }

        [Test]
        public void RepositoryName_ConValorConfigurado_RetornaValor() {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["Repository:Name"] = "PostgreSQL"
            });

            //Act
            var nombre = Configuracion.RepositoryName;

            //Assert
            nombre.Should().Be("PostgreSQL");
        }

        [Test]
        public void RepositoryName_SinValorConfigurado_RetornaSQLite() {
            //Act
            var nombre = Configuracion.RepositoryName;

            //Assert
            nombre.Should().Be("SQLite");
        }

        [Test]
        public void DbConnection_ConValorConfigurado_RetornaValor() {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["Repository:ConnectionString"] = "Data Source=usuarios.db"
            });

            //Act
            var conexion = Configuracion.DbConnection;

            //Assert
            conexion.Should().Be("Data Source=usuarios.db");
        }

        [Test]
        public void CacheName_ConValorConfigurado_RetornaValor() {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["Cache:Name"] = "Redis"
            });

            //Act
            var nombre = Configuracion.CacheName;

            //Assert
            nombre.Should().Be("Redis");
        }

        [Test]
        public void CacheName_SinValorConfigurado_RetornaMemory() {
            //Act
            var nombre = Configuracion.CacheName;

            //Assert
            nombre.Should().Be("Memory");
        }

        [Test]
        public void CacheConnectionString_ConValorConfigurado_RetornaValor() {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["Cache:ConnectionString"] = "localhost:6379"
            });

            //Act
            var conexion = Configuracion.CacheConnectionString;

            //Assert
            conexion.Should().Be("localhost:6379");
        }

        [TestCase("1", 1)]
        [TestCase("30", 30)]
        [TestCase("120", 120)]
        public void CacheTtl_ConValorConfigurado_RetornaValor(string valor, int esperado) {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["Cache:TTL"] = valor
            });

            //Act
            var ttl = Configuracion.CacheTtl;

            //Assert
            ttl.Should().Be(esperado);
        }

        [Test]
        public void CacheTtl_SinValorConfigurado_RetornaTreinta() {
            //Act
            var ttl = Configuracion.CacheTtl;

            //Assert
            ttl.Should().Be(30);
        }

        [TestCase("1", 1)]
        [TestCase("60", 60)]
        [TestCase("180", 180)]
        public void CacheSincronizacion_ConValorConfigurado_RetornaValor(string valor, int esperado) {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["Cache:Sincronización"] = valor
            });

            //Act
            var sincronizacion = Configuracion.CacheSincronizacion;

            //Assert
            sincronizacion.Should().Be(esperado);
        }

        [Test]
        public void CacheSincronizacion_SinValorConfigurado_RetornaSesenta() {
            //Act
            var sincronizacion = Configuracion.CacheSincronizacion;

            //Assert
            sincronizacion.Should().Be(60);
        }

        [Test]
        public void DataFolder_RetornaRutaDataDelDirectorioActual() {
            //Arrange
            var rutaEsperada = Path.Combine(Directory.GetCurrentDirectory(), "data");

            //Act
            var carpeta = Configuracion.DataFolder;

            //Assert
            carpeta.Should().Be(rutaEsperada);
            Path.IsPathRooted(carpeta).Should().BeTrue();
        }

        [Test]
        public void UsersJsonPath_RetornaRutaDelArchivoUsuarios() {
            //Arrange
            var rutaEsperada = Path.Combine(Directory.GetCurrentDirectory(), "data", "users.json");

            //Act
            var archivo = Configuracion.UsersJsonPath;

            //Assert
            archivo.Should().Be(rutaEsperada);
            Path.IsPathRooted(archivo).Should().BeTrue();
            Path.GetFileName(archivo).Should().Be("users.json");
        }

        [TestCase(null, "development")]
        [TestCase("Development", "development")]
        [TestCase("Production", "production")]
        [TestCase("PrOdUcTiOn", "production")]
        public void Inicializar_ConArchivoValido_CargaConfiguracion(string? argumento, string entorno) {
            //Arrange
            var argumentos = argumento is null
                ? Array.Empty<string>()
                : new[] { argumento };

            var configuracionAnterior = Configuracion.Configuration;

            CrearArchivoConfiguracion(entorno, """
                {
                    "ApiSettings": {
                        "Name": "ApiPruebas",
                        "BaseUrl": "https://localhost:7000"
                    },
                    "Repository": {
                        "Name": "SQLite",
                        "ConnectionString": "Data Source=pruebas.db"
                    },
                    "Cache": {
                        "Name": "Redis",
                        "ConnectionString": "localhost:6379",
                        "TTL": 120,
                        "Sincronización": 90
                    }
                }
                """);

            //Act
            Configuracion.Inicializar(argumentos);

            //Assert
            Configuracion.Configuration.Should().NotBeNull();
            Configuracion.Configuration.Should().NotBeSameAs(configuracionAnterior);
            Configuracion.ApiName.Should().Be("ApiPruebas");
            Configuracion.BaseUrl.Should().Be("https://localhost:7000");
            Configuracion.RepositoryName.Should().Be("SQLite");
            Configuracion.DbConnection.Should().Be("Data Source=pruebas.db");
            Configuracion.CacheName.Should().Be("Redis");
            Configuracion.CacheConnectionString.Should().Be("localhost:6379");
            Configuracion.CacheTtl.Should().Be(120);
            Configuracion.CacheSincronizacion.Should().Be(90);
        }

        [Test]
        public void Inicializar_ConJsonSinPropiedades_UtilizaValoresPorDefecto() {
            //Arrange
            var entorno = $"test_{Guid.NewGuid():N}";

            CrearArchivoConfiguracion(entorno, "{}");

            //Act
            Configuracion.Inicializar(new[] { entorno });

            //Assert
            Configuracion.Configuration.Should().NotBeNull();
            Configuracion.ApiName.Should().Be("Development");
            Configuracion.BaseUrl.Should().Be("https://jsonplaceholder.typicode.com");
            Configuracion.RepositoryName.Should().Be("SQLite");
            Configuracion.CacheName.Should().Be("Memory");
            Configuracion.CacheTtl.Should().Be(30);
            Configuracion.CacheSincronizacion.Should().Be(60);
        }

        [Test]
        public void Inicializar_ConVariosArgumentos_UtilizaElPrimero() {
            //Arrange
            var entorno = $"test_{Guid.NewGuid():N}";

            CrearArchivoConfiguracion(entorno, """
                {
                    "ApiSettings": {
                        "Name": "PrimerArgumento"
                    }
                }
                """);

            //Act
            Configuracion.Inicializar(new[] { entorno, "otro_entorno" });

            //Assert
            Configuracion.ApiName.Should().Be("PrimerArgumento");
        }
    }

    [TestFixture]
    [NonParallelizable]
    public sealed class CasosNoValidos : ConfiguracionTestsBase {

        [Test]
        public void DbConnection_SinValorConfigurado_LanzaExcepcion() {
            //Act
            Action accion = () => {
                _ = Configuracion.DbConnection;
            };

            //Assert
            accion.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Falta Repository:ConnectionString en la configuración.");
        }

        [Test]
        public void CacheConnectionString_SinValorConfigurado_LanzaExcepcion() {
            //Act
            Action accion = () => {
                _ = Configuracion.CacheConnectionString;
            };

            //Assert
            accion.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Falta Cache:ConnectionString en la configuración.");
        }

        [TestCase("abc")]
        [TestCase("12.5")]
        [TestCase("2147483648")]
        public void CacheTtl_ConValorNoConvertibleAEntero_LanzaExcepcion(string valor) {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["Cache:TTL"] = valor
            });

            //Act
            Action accion = () => {
                _ = Configuracion.CacheTtl;
            };

            //Assert
            accion.Should().Throw<InvalidOperationException>();
        }

        [TestCase("abc")]
        [TestCase("12.5")]
        [TestCase("2147483648")]
        public void CacheSincronizacion_ConValorNoConvertibleAEntero_LanzaExcepcion(string valor) {
            //Arrange
            EstablecerConfiguracion(new Dictionary<string, string?> {
                ["Cache:Sincronización"] = valor
            });

            //Act
            Action accion = () => {
                _ = Configuracion.CacheSincronizacion;
            };

            //Assert
            accion.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Inicializar_ConArchivoInexistente_LanzaExcepcion() {
            //Arrange
            var entorno = $"inexistente_{Guid.NewGuid():N}";
            var configuracionAnterior = Configuracion.Configuration;

            //Act
            Action accion = () => Configuracion.Inicializar(new[] { entorno });

            //Assert
            accion.Should().Throw<FileNotFoundException>();
            Configuracion.Configuration.Should().BeSameAs(configuracionAnterior);
        }

        [Test]
        public void Inicializar_ConJsonIncorrecto_LanzaExcepcion() {
            //Arrange
            var entorno = $"incorrecto_{Guid.NewGuid():N}";
            var configuracionAnterior = Configuracion.Configuration;

            CrearArchivoConfiguracion(entorno, "{ \"ApiSettings\": ");

            //Act
            Action accion = () => Configuracion.Inicializar(new[] { entorno });

            //Assert
            accion.Should().Throw<InvalidDataException>();
            Configuracion.Configuration.Should().BeSameAs(configuracionAnterior);
        }
    }
}