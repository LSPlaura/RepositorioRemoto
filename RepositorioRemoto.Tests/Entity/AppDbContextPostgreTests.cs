using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Back.Entity;
using RepositorioRemoto.Back.Models;

namespace RepositorioRemoto.Tests.Entity;

[TestFixture]
public class AppDbContextPostgreTests {

    [TestFixture]
    public sealed class CasosValidos {

        private AppDbContextPostgre _context = null!;

        [SetUp]
        public void Setup() {
            var options = new DbContextOptionsBuilder<AppDbContextPostgre>()
                .UseNpgsql(
                    "Host=localhost;Database=test;Username=test;Password=test"
                )
                .Options;

            _context = new AppDbContextPostgre(options);
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
        public void Address_TieneConversionJsonConfigurada() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Address));

            // Assert
            property.Should().NotBeNull();
            property!.GetValueConverter().Should().NotBeNull();
            property.GetColumnType().Should().Be("jsonb");
        }

        [Test]
        public void Company_TieneConversionJsonConfigurada() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var property = entityType!.FindProperty(nameof(User.Company));

            // Assert
            property.Should().NotBeNull();
            property!.GetValueConverter().Should().NotBeNull();
            property.GetColumnType().Should().Be("jsonb");
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        private AppDbContextPostgre _context = null!;

        [SetUp]
        public void Setup() {
            var options = new DbContextOptionsBuilder<AppDbContextPostgre>()
                .UseNpgsql(
                    "Host=localhost;Database=test;Username=test;Password=test"
                )
                .Options;

            _context = new AppDbContextPostgre(options);
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