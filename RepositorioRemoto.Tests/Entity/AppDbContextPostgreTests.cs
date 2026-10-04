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
        public void Address_EstaConfiguradoComoOwned() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var navigation = entityType!.FindNavigation(nameof(User.Address));

            // Assert
            navigation.Should().NotBeNull();
            navigation!.TargetEntityType.IsOwned().Should().BeTrue();
        }

        [Test]
        public void Company_EstaConfiguradoComoOwned() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            // Act
            var navigation = entityType!.FindNavigation(nameof(User.Company));

            // Assert
            navigation.Should().NotBeNull();
            navigation!.TargetEntityType.IsOwned().Should().BeTrue();
        }

        [Test]
        public void Address_Geo_EstaConfiguradoComoOwned() {
            // Arrange
            var userEntity = _context.Model.FindEntityType(typeof(User));

            var addressNavigation =
                userEntity!.FindNavigation(nameof(User.Address));

            // Act
            var geoNavigation =
                addressNavigation!.TargetEntityType.FindNavigation("Geo");

            // Assert
            geoNavigation.Should().NotBeNull();
            geoNavigation!.TargetEntityType.IsOwned().Should().BeTrue();
        }

        [Test]
        public void Address_EstaConfiguradoComoJson() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            var navigation =
                entityType!.FindNavigation(nameof(User.Address));

            // Act
            var containerColumn =
                navigation!.TargetEntityType
                    .FindAnnotation("Relational:ContainerColumnName");

            // Assert
            containerColumn.Should().NotBeNull();
        }

        [Test]
        public void Company_EstaConfiguradoComoJson() {
            // Arrange
            var entityType = _context.Model.FindEntityType(typeof(User));

            var navigation =
                entityType!.FindNavigation(nameof(User.Company));

            // Act
            var containerColumn =
                navigation!.TargetEntityType
                    .FindAnnotation("Relational:ContainerColumnName");

            // Assert
            containerColumn.Should().NotBeNull();
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