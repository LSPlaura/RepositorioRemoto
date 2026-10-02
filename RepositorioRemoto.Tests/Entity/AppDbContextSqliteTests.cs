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
        public void Name_TieneLongitudMaxima50() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Name));

            // Assert
            property.Should().NotBeNull();
            property!.GetMaxLength().Should().Be(50);
        }

        [Test]
        public void UserName_TieneLongitudMaxima30() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.UserName));

            // Assert
            property.Should().NotBeNull();
            property!.GetMaxLength().Should().Be(30);
        }

        [Test]
        public void Email_TieneLongitudMaxima254() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Email));

            // Assert
            property.Should().NotBeNull();
            property!.GetMaxLength().Should().Be(254);
        }

        [Test]
        public void Phone_TieneLongitudMaxima15() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Phone));

            // Assert
            property.Should().NotBeNull();
            property!.GetMaxLength().Should().Be(15);
        }

        [Test]
        public void Website_TieneLongitudMaxima250() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Website));

            // Assert
            property.Should().NotBeNull();
            property!.GetMaxLength().Should().Be(250);
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