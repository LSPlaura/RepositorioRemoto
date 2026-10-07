using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Back.Entity;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Tests.Entity;

[TestFixture]
public class AppDbContextSqliteTests {

    [TestFixture]
    public sealed class CasosValidos {

        private AppDbContextSqlite _context = null!;

        [SetUp]
        public void Setup() {
            var options = new DbContextOptionsBuilder<AppDbContextSqlite>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            _context = new AppDbContextSqlite(options);
        }

        [TearDown]
        public void TearDown() {
            _context.Dispose();
        }

        [Test]
        public void Users_RetornaDbSetCorrectamente() {
            // Act
            var res = _context.Users;

            // Assert
            res.Should().NotBeNull();
        }

        [Test]
        public void User_SeMapeaATablaUsers() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var tableName = entityType!.GetTableName();

            // Assert
            entityType.Should().NotBeNull();
            tableName.Should().Be("users");
        }

        [Test]
        public void User_Id_EsClavePrimaria() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var primaryKey = entityType!.FindPrimaryKey();

            // Assert
            primaryKey.Should().NotBeNull();
            primaryKey!.Properties.Should().ContainSingle();
            primaryKey.Properties[0].Name.Should().Be(nameof(User.Id));
        }

        [Test]
        public void Address_TieneConversionConfigurada() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Address));
            var converter = property!.GetValueConverter();

            // Assert
            property.Should().NotBeNull();
            converter.Should().NotBeNull();
        }

        [Test]
        public void Company_TieneConversionConfigurada() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Company));
            var converter = property!.GetValueConverter();

            // Assert
            property.Should().NotBeNull();
            converter.Should().NotBeNull();
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        private AppDbContextSqlite _context = null!;

        [SetUp]
        public void Setup() {
            var options = new DbContextOptionsBuilder<AppDbContextSqlite>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            _context = new AppDbContextSqlite(options);
        }

        [TearDown]
        public void TearDown() {
            _context.Dispose();
        }

        [Test]
        public void Email_Nulo_NoEstaPermitido() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Email));

            // Assert
            property.Should().NotBeNull();
            property!.IsNullable.Should().BeFalse();
        }

        [Test]
        public void Phone_Nulo_NoEstaPermitido() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Phone));

            // Assert
            property.Should().NotBeNull();
            property!.IsNullable.Should().BeFalse();
        }
    }
}