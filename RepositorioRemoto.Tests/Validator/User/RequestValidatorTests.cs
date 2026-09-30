using FluentAssertions;
using NUnit.Framework;
using RepositorioRemoto.Back.Dto;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Dto.Users.Request;
using RepositorioRemoto.Back.Validator.User;

namespace RepositorioRemoto.Tests.Validator.User;

[TestFixture]
public class RequestValidatorTests
{
    private readonly RequestValidator _validator = new();

    private static AddressDto CrearAddressValida() => new(
        "Calle Mayor 123",
        "Piso 2 B",
        "Madrid",
        "28001",
        new GeoDto("-40.4167754", "-3.7037902")
    );

    private static CompanyDto CrearCompanyValida() => new(
        "Acme Corp",
        "Innovacion constante",
        "Soluciones tecnologicas"
    );

    private static CreateUserRequest CrearRequestValido() => new(
        "Juan Perez",
        "juan.perez",
        "juan.perez@email.com",
        CrearAddressValida(),
        "+34 600-000-000",
        "https://www.ejemplo.com",
        CrearCompanyValida()
    );

    [TestFixture]
    public class CasosValidos
    {
        private readonly RequestValidator _validator = new();

        [Test]
        public void Validate_RequestConTodosLosCamposValidos_DebeSerExitoso()
        {
            // Arrange
            var request = CrearRequestValido();

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
        }
        
        [TestCase("Ab")] // Min exacto: 2 chars
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Max exacto: 50 chars
        [TestCase("Juan A. Perez-Gomez")] // Permite espacios, puntos y guiones (sin números)
        public void Validate_NameEnValoresLimiteValidos_DebeSerExitoso(string name)
        {
            // Arrange
            var request = CrearRequestValido() with { Name = name };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
        
        [TestCase("abc")] // Min exacto: 3 chars
        [TestCase("123456789012345678901234567890")] // Max exacto: 30 chars
        [TestCase("user.name_123-test")] // Permite alfanuméricos, '.', '_', '-'
        public void Validate_UserNameEnValoresLimiteValidos_DebeSerExitoso(string userName)
        {
            // Arrange
            var request = CrearRequestValido() with { UserName = userName };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
        
        [TestCase("usuario@dominio.com")]
        [TestCase("user.name+tag@sub.domain.co.uk")]
        [TestCase("123_test@empresa.es")]
        public void Validate_EmailFormatoValido_DebeSerExitoso(string email)
        {
            // Arrange
            var request = CrearRequestValido() with { Email = email };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
        
        [TestCase("123456")] // Formato básico (mínimo de grupos)
        [TestCase("+34 600-000-000")] // Prefijo de país y separadores
        [TestCase("(91) 234-5678")] // Con paréntesis
        [TestCase("912345678 ext 1234")] // Con extensión
        public void Validate_PhoneFormatoValido_DebeSerExitoso(string phone)
        {
            // Arrange
            var request = CrearRequestValido() with { Phone = phone };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
        
        [TestCase("ejemplo.com")] // Sin protocolo ni www
        [TestCase("www.ejemplo.org")] // Con www
        [TestCase("http://ejemplo.net/path/to/page")] // Con http y ruta
        [TestCase("https://sub.domain.com/index.html?id=1")] // Con https, ruta y query
        public void Validate_WebsiteFormatoValido_DebeSerExitoso(string website)
        {
            // Arrange
            var request = CrearRequestValido() with { Website = website };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosInvalidos
    {
        private readonly RequestValidator _validator = new();
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Validate_NameVacioOEspacios_DebeRetornarFallo(string? name)
        {
            // Arrange
            var request = CrearRequestValido() with { Name = name! };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Validate_UserNameVacioOEspacios_DebeRetornarFallo(string? userName)
        {
            // Arrange
            var request = CrearRequestValido() with { UserName = userName! };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Validate_EmailVacioOEspacios_DebeRetornarFallo(string? email)
        {
            // Arrange
            var request = CrearRequestValido() with { Email = email! };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Validate_PhoneVacioOEspacios_DebeRetornarFallo(string? phone)
        {
            // Arrange
            var request = CrearRequestValido() with { Phone = phone! };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Validate_WebsiteVacioOEspacios_DebeRetornarFallo(string? website)
        {
            // Arrange
            var request = CrearRequestValido() with { Website = website! };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [Test]
        public void Validate_AddressCamposVacios_DebeRetornarFalloDeAddress()
        {
            // Arrange: Address con campo Street vacío
            var addressInvalida = CrearAddressValida() with { Street = "" };
            var request = CrearRequestValido() with { Address = addressInvalida };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_CompanyCamposVacios_DebeRetornarFalloDeCompany()
        {
            // Arrange: Company con campo Name vacío
            var companyInvalida = CrearCompanyValida() with { Name = "" };
            var request = CrearRequestValido() with { Company = companyInvalida };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("A")] // Justo debajo del Min (1 char)
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Justo sobre el Max (51 chars)
        [TestCase("Juan123")] // Caracter no permitido: Números
        [TestCase("Juan@Perez")] // Caracter no permitido: '@'
        [TestCase("Juan_Perez")] // Caracter no permitido: '_'
        [TestCase("Juan!Perez")] // Caracter no permitido: '!'
        public void Validate_NameFueraDeLimitesOCaracterNoPermitido_DebeRetornarFallo(string name)
        {
            // Arrange
            var request = CrearRequestValido() with { Name = name };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("ab")] // Justo debajo del Min (2 chars)
        [TestCase("1234567890123456789012345678901")] // Justo sobre el Max (31 chars)
        [TestCase("user name")] // Caracter no permitido: Espacios
        [TestCase("user@name")] // Caracter no permitido: '@'
        [TestCase("user!name")] // Caracter no permitido: '!'
        [TestCase("user#name")] // Caracter no permitido: '#'
        public void Validate_UserNameFueraDeLimitesOCaracterNoPermitido_DebeRetornarFallo(string userName)
        {
            // Arrange
            var request = CrearRequestValido() with { UserName = userName };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("usuario")] // Sin '@' ni dominio
        [TestCase("usuario@")] // Sin dominio
        [TestCase("usuario@dominio")] // Sin TLD (.com, .es)
        [TestCase("usuario@dominio.c")] // TLD de 1 solo carácter (requiere mínimo 2)
        [TestCase("usuario con espacios@dominio.com")] // Espacios en el usuario
        [TestCase("@dominio.com")] // Sin nombre de usuario
        public void Validate_EmailFormatoIncorrecto_DebeRetornarFallo(string email)
        {
            // Arrange
            var request = CrearRequestValido() with { Email = email };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("abc-def-ghi")] // Letras no permitidas
        [TestCase("12345@6789")] // Símbolo no permitido '@'
        [TestCase("++34 600000000")] // Prefijo no válido
        public void Validate_PhoneFormatoIncorrecto_DebeRetornarFallo(string phone)
        {
            // Arrange
            var request = CrearRequestValido() with { Phone = phone };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("http://")] // Protocolo sin dominio
        [TestCase("http://.com")] // Sin nombre de dominio
        [TestCase("sitio.c")] // TLD de 1 carácter (mínimo 2)
        [TestCase("http://sitio con espacios.com")] // Espacios en la URL
        [TestCase("ftp://sitio.com")] // Protocolo no soportado (solo http/https)
        public void Validate_WebsiteFormatoIncorrecto_DebeRetornarFallo(string website)
        {
            // Arrange
            var request = CrearRequestValido() with { Website = website };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [Test]
        public void Validate_AddressFormatoInvalido_DebeRetornarFalloDeAddress()
        {
            // Arrange: Address con formato de código postal inválido (2 letras)
            var addressInvalida = CrearAddressValida() with { ZipCode = "AB" };
            var request = CrearRequestValido() with { Address = addressInvalida };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Validate_CompanyFormatoInvalido_DebeRetornarFalloDeCompany()
        {
            // Arrange: Company con caracter invalido en Name
            var companyInvalida = CrearCompanyValida() with { Name = "Company@Corp" };
            var request = CrearRequestValido() with { Company = companyInvalida };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}