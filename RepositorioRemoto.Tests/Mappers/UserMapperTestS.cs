using System;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Mappers;

namespace RepositorioRemoto.Tests.Mappers;

[TestFixture]
public class UserMapperTests {

    private const string JsonUsuario = """
        {
            "id": 25,
            "name": "Laura García",
            "userName": "laura",
            "email": "laura@example.com",
            "address": {
                "street": "Calle Mayor",
                "suite": "Piso 2",
                "city": "Madrid",
                "zipCode": "28001",
                "geo": {
                    "lat": "40.4168",
                    "lng": "-3.7038"
                }
            },
            "phone": "600123456",
            "website": "laura.example.com",
            "company": {
                "name": "Empresa de pruebas",
                "catchPhrase": "Tecnología para todos",
                "bs": "desarrollo de software"
            }
        }
        """;

    private static T CrearDto<T>(string json) {
        return JsonSerializer.Deserialize<T>(
            json,
            new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            }
        )!;
    }

    [TestFixture]
    public sealed class CasosValidos {

        private CreateUserRequest _createDto = null!;
        private UpdateUserRequest _updateDto = null!;

        [SetUp]
        public void Setup() {
            _createDto = CrearDto<CreateUserRequest>(JsonUsuario);
            _updateDto = CrearDto<UpdateUserRequest>(JsonUsuario);
        }

        [Test]
        public void ToModel_CreateUserRequestValido_CopiaDatosDelUsuario() {
            //Act
            var resultado = _createDto.ToModel();

            //Assert
            resultado.Should().NotBeNull();
            resultado.Name.Should().Be("Laura García");
            resultado.UserName.Should().Be("laura");
            resultado.Email.Should().Be("laura@example.com");
            resultado.Phone.Should().Be("600123456");
            resultado.Website.Should().Be("laura.example.com");
        }

        [Test]
        public void ToModel_CreateUserRequestValido_AsignaIdCero() {
            //Act
            var resultado = _createDto.ToModel();

            //Assert
            resultado.Id.Should().Be(0);
        }

        [Test]
        public void ToModel_CreateUserRequestValido_MapeaAddressYGeo() {
            //Act
            var resultado = _createDto.ToModel();

            //Assert
            resultado.Address.Should().NotBeNull();
            resultado.Address.Should().NotBeSameAs(_createDto.Address);

            resultado.Address.Street.Should().Be("Calle Mayor");
            resultado.Address.Suite.Should().Be("Piso 2");
            resultado.Address.City.Should().Be("Madrid");
            resultado.Address.ZipCode.Should().Be("28001");

            resultado.Address.Geo.Should().NotBeNull();
            resultado.Address.Geo.Should().NotBeSameAs(_createDto.Address.Geo);
            resultado.Address.Geo.Lat.Should().Be("40.4168");
            resultado.Address.Geo.Lng.Should().Be("-3.7038");
        }

        [Test]
        public void ToModel_CreateUserRequestValido_MapeaCompany() {
            //Act
            var resultado = _createDto.ToModel();

            //Assert
            resultado.Company.Should().NotBeNull();
            resultado.Company.Should().NotBeSameAs(_createDto.Company);

            resultado.Company.Name.Should().Be("Empresa de pruebas");
            resultado.Company.CatchPhrase.Should().Be("Tecnología para todos");
            resultado.Company.Bs.Should().Be("desarrollo de software");
        }

        [Test]
        public void ToModel_CreateUserRequestValido_AsignaFechasActualesUtc() {
            //Arrange
            var antes = DateTime.UtcNow;

            //Act
            var resultado = _createDto.ToModel();
            var despues = DateTime.UtcNow;

            //Assert
            resultado.CreateAt.Should().BeOnOrAfter(antes);
            resultado.CreateAt.Should().BeOnOrBefore(despues);
            resultado.CreateAt.Kind.Should().Be(DateTimeKind.Utc);

            resultado.UpdateAt.Should().BeOnOrAfter(antes);
            resultado.UpdateAt.Should().BeOnOrBefore(despues);
            resultado.UpdateAt.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Test]
        public void ToModel_CreateUserRequestValido_InicializaEstadoSinBorrar() {
            //Act
            var resultado = _createDto.ToModel();

            //Assert
            resultado.IsDeleted.Should().BeFalse();
            resultado.DeleteAt.Should().Be(default);
        }

        [Test]
        public void ToModel_UpdateUserRequestValido_CopiaIdYDatosDelUsuario() {
            //Act
            var resultado = _updateDto.ToModel();

            //Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(25);
            resultado.Name.Should().Be("Laura García");
            resultado.UserName.Should().Be("laura");
            resultado.Email.Should().Be("laura@example.com");
            resultado.Phone.Should().Be("600123456");
            resultado.Website.Should().Be("laura.example.com");
        }

        [Test]
        public void ToModel_UpdateUserRequestValido_MapeaAddressYGeo() {
            //Act
            var resultado = _updateDto.ToModel();

            //Assert
            resultado.Address.Should().NotBeNull();
            resultado.Address.Should().NotBeSameAs(_updateDto.Address);

            resultado.Address.Street.Should().Be("Calle Mayor");
            resultado.Address.Suite.Should().Be("Piso 2");
            resultado.Address.City.Should().Be("Madrid");
            resultado.Address.ZipCode.Should().Be("28001");

            resultado.Address.Geo.Should().NotBeNull();
            resultado.Address.Geo.Should().NotBeSameAs(_updateDto.Address.Geo);
            resultado.Address.Geo.Lat.Should().Be("40.4168");
            resultado.Address.Geo.Lng.Should().Be("-3.7038");
        }

        [Test]
        public void ToModel_UpdateUserRequestValido_MapeaCompany() {
            //Act
            var resultado = _updateDto.ToModel();

            //Assert
            resultado.Company.Should().NotBeNull();
            resultado.Company.Should().NotBeSameAs(_updateDto.Company);

            resultado.Company.Name.Should().Be("Empresa de pruebas");
            resultado.Company.CatchPhrase.Should().Be("Tecnología para todos");
            resultado.Company.Bs.Should().Be("desarrollo de software");
        }

        [Test]
        public void ToModel_UpdateUserRequestValido_AsignaFechaActualizacionUtc() {
            //Arrange
            var antes = DateTime.UtcNow;

            //Act
            var resultado = _updateDto.ToModel();
            var despues = DateTime.UtcNow;

            //Assert
            resultado.UpdateAt.Should().BeOnOrAfter(antes);
            resultado.UpdateAt.Should().BeOnOrBefore(despues);
            resultado.UpdateAt.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Test]
        public void ToModel_CreateUserRequest_CreaInstanciasIndependientes() {
            //Act
            var primero = _createDto.ToModel();
            var segundo = _createDto.ToModel();

            //Assert
            primero.Should().NotBeSameAs(segundo);
            primero.Address.Should().NotBeSameAs(segundo.Address);
            primero.Address.Geo.Should().NotBeSameAs(segundo.Address.Geo);
            primero.Company.Should().NotBeSameAs(segundo.Company);
        }

        [Test]
        public void ToModel_UpdateUserRequest_CreaInstanciasIndependientes() {
            //Act
            var primero = _updateDto.ToModel();
            var segundo = _updateDto.ToModel();

            //Assert
            primero.Should().NotBeSameAs(segundo);
            primero.Address.Should().NotBeSameAs(segundo.Address);
            primero.Address.Geo.Should().NotBeSameAs(segundo.Address.Geo);
            primero.Company.Should().NotBeSameAs(segundo.Company);
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        [Test]
        public void ToModel_CreateUserRequestNulo_LanzaNullReferenceException() {
            //Arrange
            CreateUserRequest dto = null!;

            //Act
            Action accion = () => {
                _ = dto.ToModel();
            };

            //Assert
            accion.Should().Throw<NullReferenceException>();
        }

        [Test]
        public void ToModel_UpdateUserRequestNulo_LanzaNullReferenceException() {
            //Arrange
            UpdateUserRequest dto = null!;

            //Act
            Action accion = () => {
                _ = dto.ToModel();
            };

            //Assert
            accion.Should().Throw<NullReferenceException>();
        }

        [TestCase("address")]
        [TestCase("company")]
        public void ToModel_CreateUserRequestConObjetoNulo_LanzaNullReferenceException(string propiedad) {
            //Arrange
            var datos = JsonSerializer.Deserialize<
                System.Collections.Generic.Dictionary<string, JsonElement>
            >(JsonUsuario)!;

            datos[propiedad] = JsonSerializer.SerializeToElement<object?>(null);

            var dto = CrearDto<CreateUserRequest>(JsonSerializer.Serialize(datos));

            //Act
            Action accion = () => {
                _ = dto.ToModel();
            };

            //Assert
            accion.Should().Throw<NullReferenceException>();
        }

        [TestCase("address")]
        [TestCase("company")]
        public void ToModel_UpdateUserRequestConObjetoNulo_LanzaNullReferenceException(string propiedad) {
            //Arrange
            var datos = JsonSerializer.Deserialize<
                System.Collections.Generic.Dictionary<string, JsonElement>
            >(JsonUsuario)!;

            datos[propiedad] = JsonSerializer.SerializeToElement<object?>(null);

            var dto = CrearDto<UpdateUserRequest>(JsonSerializer.Serialize(datos));

            //Act
            Action accion = () => {
                _ = dto.ToModel();
            };

            //Assert
            accion.Should().Throw<NullReferenceException>();
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ToModel_UpdateUserRequestConIdNoPositivo_ConservaId(int id) {
            //Arrange
            var json = JsonUsuario.Replace("\"id\": 25", $"\"id\": {id}");
            var dto = CrearDto<UpdateUserRequest>(json);

            //Act
            var resultado = dto.ToModel();

            //Assert
            resultado.Id.Should().Be(id);
        }

        [TestCase("")]
        [TestCase("correo-invalido")]
        public void ToModel_CreateUserRequestConEmailInvalido_ConservaEmail(string email) {
            //Arrange
            var json = JsonUsuario.Replace("laura@example.com", email);
            var dto = CrearDto<CreateUserRequest>(json);

            //Act
            var resultado = dto.ToModel();

            //Assert
            resultado.Email.Should().Be(email);
        }

        [TestCase("")]
        [TestCase("correo-invalido")]
        public void ToModel_UpdateUserRequestConEmailInvalido_ConservaEmail(string email) {
            //Arrange
            var json = JsonUsuario.Replace("laura@example.com", email);
            var dto = CrearDto<UpdateUserRequest>(json);

            //Act
            var resultado = dto.ToModel();

            //Assert
            resultado.Email.Should().Be(email);
        }
    }
}