using FluentAssertions;
using NUnit.Framework;
using RepositorioRemoto.Back.Dto.Users;
using RepositorioRemoto.Back.Validator.User;

namespace RepositorioRemoto.Tests.Validator.User;

[TestFixture]
public class CompanyValidatorTests
{
    [TestFixture]
    public class CasosValidos
    {
        [Test]
        public void CheckEmptyOrWhiteSpace_EmpresaConCamposValidos_DebeSerExitoso()
        {
            // Arrange
            var company = new CompanyDto(
                "Acme Corp",
                "Innovacion constante",
                "Soluciones tecnologicas"
            );

            // Act
            var result = company.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
        }

        // Limite inferior exacto (Min) y superior exacto (Max) para Name (2 - 100 caracteres)
        [TestCase("Ab")] // Min exacto: 2 chars
        [TestCase("AB")] // Min exacto: 2 chars
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Max exacto: 100 chars
        [TestCase("Empresa 123, Inc.#-")] // Todos los caracteres especiales permitidos (.,#-)
        public void CheckRegex_NameEnValoresLimiteValidos_DebeSerExitoso(string name)
        {
            // Arrange
            var company = new CompanyDto(name, "Eslogan valido", "Sector valido");

            // Act
            var result = company.CheckRegex();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        // Limite inferior exacto (Min) y superior exacto (Max) para CatchPhrase (3 - 150 caracteres)
        [TestCase("Abc")] // Min exacto: 3 chars
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Max exacto: 150 chars
        [TestCase("Eslogan con 123, .#-")] // Caracteres especiales permitidos
        public void CheckRegex_CatchPhraseEnValoresLimiteValidos_DebeSerExitoso(string catchPhrase)
        {
            // Arrange
            var company = new CompanyDto("Empresa Valida", catchPhrase, "Sector valido");

            // Act
            var result = company.CheckRegex();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        // Limite inferior exacto (Min) y superior exacto (Max) para Bs (3 - 150 caracteres)
        [TestCase("Bss")] // Min exacto: 3 chars
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Max exacto: 150 chars
        [TestCase("Actividad 123, .#-")] // Caracteres especiales permitidos
        public void CheckRegex_BsEnValoresLimiteValidos_DebeSerExitoso(string bs)
        {
            // Arrange
            var company = new CompanyDto("Empresa Valida", "Eslogan valido", bs);

            // Act
            var result = company.CheckRegex();

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosInvalidos
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void CheckEmptyOrWhiteSpace_NameVacioOEspacios_DebeRetornarFallo(string? name)
        {
            // Arrange
            var company = new CompanyDto(name!, "Eslogan valido", "Sector valido");

            // Act
            var result = company.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void CheckEmptyOrWhiteSpace_CatchPhraseVacioOEspacios_DebeRetornarFallo(string? catchPhrase)
        {
            // Arrange
            var company = new CompanyDto("Empresa Valida", catchPhrase!, "Sector valido");

            // Act
            var result = company.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void CheckEmptyOrWhiteSpace_BsVacioOEspacios_DebeRetornarFallo(string? bs)
        {
            // Arrange
            var company = new CompanyDto("Empresa Valida", "Eslogan valido", bs!);

            // Act
            var result = company.CheckEmptyOrWhiteSpace();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("A")] // Justo debajo del Min (1 char)
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Justo sobre el Max (101 chars)
        [TestCase("Empresa@Corp")] // Caracter no permitido '@'
        [TestCase("Empresa!")] // Caracter no permitido '!'
        [TestCase("Empresa$")] // Caracter no permitido '$'
        [TestCase("Empresa%")] // Caracter no permitido '%'
        [TestCase("Empresa&")] // Caracter no permitido '&'
        [TestCase("Empresa*")] // Caracter no permitido '*'
        [TestCase("Empresa_")] // Caracter no permitido '_'
        public void CheckRegex_NameFueraDeLimitesOCaracterNoPermitido_DebeRetornarFallo(string name)
        {
            // Arrange
            var company = new CompanyDto(name, "Eslogan valido", "Sector valido");

            // Act
            var result = company.CheckRegex();

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [TestCase("Ab")] // Justo debajo del Min (2 chars)
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Exactamente 151 chars (151 'a')
        [TestCase("Eslogan con @")] // Caracter no permitido '@'
        [TestCase("Eslogan con !")] // Caracter no permitido '!'
        [TestCase("Eslogan con $")] // Caracter no permitido '$'
        [TestCase("Eslogan con &")] // Caracter no permitido '&'
        [TestCase("Eslogan con _")] // Caracter no permitido '_'
        public void CheckRegex_CatchPhraseFueraDeLimitesOCaracterNoPermitido_DebeRetornarFallo(string catchPhrase)
        {
            // Arrange
            var company = new CompanyDto("Empresa Valida", catchPhrase, "Sector valido");

            // Act
            var result = company.CheckRegex();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [TestCase("Bs")] // Justo debajo del Min (2 chars)
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // Exactamente 151 chars (151 'a')
        [TestCase("Sector con @")] // Caracter no permitido '@'
        [TestCase("Sector con !")] // Caracter no permitido '!'
        [TestCase("Sector con $")] // Caracter no permitido '$'
        [TestCase("Sector con %")] // Caracter no permitido '%'
        [TestCase("Sector con &")] // Caracter no permitido '&'
        [TestCase("Sector con _")] // Caracter no permitido '_'
        public void CheckRegex_BsFueraDeLimitesOCaracterNoPermitido_DebeRetornarFallo(string bs)
        {
            // Arrange
            var company = new CompanyDto("Empresa Valida", "Eslogan valido", bs);

            // Act
            var result = company.CheckRegex();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}