using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using InMemoryCache = RepositorioRemoto.Back.Cache.MemoryCache;
using MicrosoftMemoryCache = Microsoft.Extensions.Caching.Memory.MemoryCache;

namespace RepositorioRemoto.Tests.Cache;

[TestFixture]
public class MemoryCacheTests {

    private sealed record UsuarioCache(int Id, string Nombre);

    [TestFixture]
    public sealed class CasosValidos {

        private Mock<IMemoryCache> _memoryCache = null!;
        private InMemoryCache _cache = null!;

        [SetUp]
        public void SetUp() {
            _memoryCache = new Mock<IMemoryCache>();
            _cache = new InMemoryCache(_memoryCache.Object);
        }

        [Test]
        public async Task GetAsync_ConClaveExistente_RetornaValorGuardado() {
            //Arrange
            var usuario = new UsuarioCache(1, "Laura");
            object? valorGuardado = usuario;

            _memoryCache
                .Setup(cache => cache.TryGetValue("usuario:1", out valorGuardado))
                .Returns(true);

            //Act
            var resultado = await _cache.GetAsync<UsuarioCache>("usuario:1");

            //Assert
            resultado.Should().BeEquivalentTo(usuario);

            _memoryCache.Verify(
                cache => cache.TryGetValue("usuario:1", out valorGuardado),
                Times.Once
            );
        }

        [Test]
        public async Task GetAsync_ConClaveInexistente_RetornaNull() {
            //Arrange
            object? valorGuardado = null;

            _memoryCache
                .Setup(cache => cache.TryGetValue("inexistente", out valorGuardado))
                .Returns(false);

            //Act
            var resultado = await _cache.GetAsync<UsuarioCache>("inexistente");

            //Assert
            resultado.Should().BeNull();

            _memoryCache.Verify(
                cache => cache.TryGetValue("inexistente", out valorGuardado),
                Times.Once
            );
        }

        [Test]
        public async Task GetAsync_ConEnteroGuardado_RetornaEntero() {
            //Arrange
            object? valorGuardado = 25;

            _memoryCache
                .Setup(cache => cache.TryGetValue("numero", out valorGuardado))
                .Returns(true);

            //Act
            var resultado = await _cache.GetAsync<int>("numero");

            //Assert
            resultado.Should().Be(25);

            _memoryCache.Verify(
                cache => cache.TryGetValue("numero", out valorGuardado),
                Times.Once
            );
        }

        [Test]
        public async Task GetAsync_ConEnteroInexistente_RetornaCero() {
            //Arrange
            object? valorGuardado = null;

            _memoryCache
                .Setup(cache => cache.TryGetValue("numero", out valorGuardado))
                .Returns(false);

            //Act
            var resultado = await _cache.GetAsync<int>("numero");

            //Assert
            resultado.Should().Be(0);

            _memoryCache.Verify(
                cache => cache.TryGetValue("numero", out valorGuardado),
                Times.Once
            );
        }

        [Test]
        public async Task SetAsync_ConExpiracion_GuardaValorYConfiguraExpiracion() {
            //Arrange
            var entrada = new Mock<ICacheEntry>();
            entrada.SetupAllProperties();

            var usuario = new UsuarioCache(1, "Laura");
            var expiracion = TimeSpan.FromMinutes(2);

            _memoryCache
                .Setup(cache => cache.CreateEntry("usuario:1"))
                .Returns(entrada.Object);

            //Act
            await _cache.SetAsync("usuario:1", usuario, expiracion);

            //Assert
            entrada.Object.Value.Should().BeSameAs(usuario);
            entrada.Object.AbsoluteExpirationRelativeToNow.Should().Be(expiracion);

            _memoryCache.Verify(
                cache => cache.CreateEntry("usuario:1"),
                Times.Once
            );

            entrada.VerifySet(
                entry => entry.Value = usuario,
                Times.Once
            );

            entrada.VerifySet(
                entry => entry.AbsoluteExpirationRelativeToNow = expiracion,
                Times.Once
            );

            entrada.Verify(entry => entry.Dispose(), Times.Once);
        }

        [Test]
        public async Task SetAsync_SinExpiracion_UtilizaCincoMinutos() {
            //Arrange
            var entrada = new Mock<ICacheEntry>();
            entrada.SetupAllProperties();

            var expiracionEsperada = TimeSpan.FromMinutes(5);

            _memoryCache
                .Setup(cache => cache.CreateEntry("clave"))
                .Returns(entrada.Object);

            //Act
            await _cache.SetAsync("clave", "valor");

            //Assert
            entrada.Object.Value.Should().Be("valor");
            entrada.Object.AbsoluteExpirationRelativeToNow.Should().Be(expiracionEsperada);

            _memoryCache.Verify(
                cache => cache.CreateEntry("clave"),
                Times.Once
            );

            entrada.VerifySet(
                entry => entry.Value = "valor",
                Times.Once
            );

            entrada.VerifySet(
                entry => entry.AbsoluteExpirationRelativeToNow = expiracionEsperada,
                Times.Once
            );

            entrada.Verify(entry => entry.Dispose(), Times.Once);
        }

        [Test]
        public async Task SetAsync_ConCacheReal_PermiteRecuperarElValor() {
            //Arrange
            using var cacheReal = new MicrosoftMemoryCache(new MemoryCacheOptions());
            var cache = new InMemoryCache(cacheReal);
            var usuario = new UsuarioCache(1, "Laura");

            //Act
            await cache.SetAsync("usuario:1", usuario);
            var resultado = await cache.GetAsync<UsuarioCache>("usuario:1");

            //Assert
            resultado.Should().BeEquivalentTo(usuario);
            cacheReal.Count.Should().Be(1);
        }

        [Test]
        public async Task SetAsync_ConClaveExistente_ActualizaElValor() {
            //Arrange
            using var cacheReal = new MicrosoftMemoryCache(new MemoryCacheOptions());
            var cache = new InMemoryCache(cacheReal);

            cacheReal.Set("clave", "anterior");

            //Act
            await cache.SetAsync("clave", "nuevo");

            //Assert
            cacheReal.Get<string>("clave").Should().Be("nuevo");
            cacheReal.Count.Should().Be(1);
        }

        [Test]
        public async Task RemoveAsync_ConClave_InvocaEliminacion() {
            //Arrange
            const string clave = "usuario:1";

            //Act
            await _cache.RemoveAsync(clave);

            //Assert
            _memoryCache.Verify(
                cache => cache.Remove(clave),
                Times.Once
            );
        }

        [Test]
        public async Task RemoveAsync_ConCacheReal_EliminaSoloLaClaveIndicada() {
            //Arrange
            using var cacheReal = new MicrosoftMemoryCache(new MemoryCacheOptions());
            var cache = new InMemoryCache(cacheReal);

            cacheReal.Set("clave:1", "valor:1");
            cacheReal.Set("clave:2", "valor:2");

            //Act
            await cache.RemoveAsync("clave:1");

            //Assert
            cacheReal.TryGetValue("clave:1", out _).Should().BeFalse();
            cacheReal.Get<string>("clave:2").Should().Be("valor:2");
            cacheReal.Count.Should().Be(1);
        }

        [Test]
        public async Task RemoveAsync_ConClaveInexistente_NoLanzaExcepcion() {
            //Arrange
            using var cacheReal = new MicrosoftMemoryCache(new MemoryCacheOptions());
            var cache = new InMemoryCache(cacheReal);

            //Act
            Func<Task> accion = () => cache.RemoveAsync("inexistente");

            //Assert
            await accion.Should().NotThrowAsync();
            cacheReal.Count.Should().Be(0);
        }

        [Test]
        public async Task RemoveAllAsync_ConCacheReal_EliminaTodasLasEntradas() {
            //Arrange
            using var cacheReal = new MicrosoftMemoryCache(new MemoryCacheOptions());
            var cache = new InMemoryCache(cacheReal);

            cacheReal.Set("clave:1", "valor:1");
            cacheReal.Set("clave:2", "valor:2");
            cacheReal.Set("clave:3", "valor:3");

            //Act
            await cache.RemoveAllAsync();

            //Assert
            cacheReal.Count.Should().Be(0);
            cacheReal.TryGetValue("clave:1", out _).Should().BeFalse();
            cacheReal.TryGetValue("clave:2", out _).Should().BeFalse();
            cacheReal.TryGetValue("clave:3", out _).Should().BeFalse();
        }

        [Test]
        public async Task RemoveAllAsync_ConCacheRealVacia_NoLanzaExcepcion() {
            //Arrange
            using var cacheReal = new MicrosoftMemoryCache(new MemoryCacheOptions());
            var cache = new InMemoryCache(cacheReal);

            //Act
            Func<Task> accion = () => cache.RemoveAllAsync();

            //Assert
            await accion.Should().NotThrowAsync();
            cacheReal.Count.Should().Be(0);
        }

        [Test]
        public async Task RemoveAllAsync_ConOtraImplementacion_NoRealizaOperaciones() {
            //Act
            Func<Task> accion = () => _cache.RemoveAllAsync();

            //Assert
            await accion.Should().NotThrowAsync();
            _memoryCache.VerifyNoOtherCalls();
        }
    }

    [TestFixture]
    public sealed class CasosInvalidosYExcepciones {

        private Mock<IMemoryCache> _memoryCache = null!;
        private InMemoryCache _cache = null!;

        [SetUp]
        public void SetUp() {
            _memoryCache = new Mock<IMemoryCache>();
            _cache = new InMemoryCache(_memoryCache.Object);
        }

        [Test]
        public async Task GetAsync_CuandoLaCacheFalla_RetornaNull() {
            //Arrange
            object? valorGuardado = null;

            _memoryCache
                .Setup(cache => cache.TryGetValue("clave", out valorGuardado))
                .Throws(new InvalidOperationException("Cache no disponible"));

            //Act
            var resultado = await _cache.GetAsync<UsuarioCache>("clave");

            //Assert
            resultado.Should().BeNull();

            _memoryCache.Verify(
                cache => cache.TryGetValue("clave", out valorGuardado),
                Times.Once
            );
        }

        [Test]
        public async Task GetAsync_CuandoLaCacheFallaConEntero_RetornaCero() {
            //Arrange
            object? valorGuardado = null;

            _memoryCache
                .Setup(cache => cache.TryGetValue("numero", out valorGuardado))
                .Throws(new InvalidOperationException("Cache no disponible"));

            //Act
            var resultado = await _cache.GetAsync<int>("numero");

            //Assert
            resultado.Should().Be(0);

            _memoryCache.Verify(
                cache => cache.TryGetValue("numero", out valorGuardado),
                Times.Once
            );
        }

        [Test]
        public async Task GetAsync_ConTipoIncompatible_RetornaNull() {
            //Arrange
            object? valorGuardado = "No es un usuario";

            _memoryCache
                .Setup(cache => cache.TryGetValue("clave", out valorGuardado))
                .Returns(true);

            //Act
            var resultado = await _cache.GetAsync<UsuarioCache>("clave");

            //Assert
            resultado.Should().BeNull();

            _memoryCache.Verify(
                cache => cache.TryGetValue("clave", out valorGuardado),
                Times.Once
            );
        }

        [Test]
        public async Task SetAsync_CuandoLaCacheFalla_NoLanzaExcepcion() {
            //Arrange
            _memoryCache
                .Setup(cache => cache.CreateEntry("clave"))
                .Throws(new InvalidOperationException("Cache no disponible"));

            //Act
            Func<Task> accion = () => _cache.SetAsync("clave", "valor");

            //Assert
            await accion.Should().NotThrowAsync();

            _memoryCache.Verify(
                cache => cache.CreateEntry("clave"),
                Times.Once
            );
        }

        [TestCase(0)]
        [TestCase(-1)]
        public async Task SetAsync_ConExpiracionNoPositiva_NoGuardaNiLanzaExcepcion(int segundos) {
            //Arrange
            var expiracion = TimeSpan.FromSeconds(segundos);

            //Act
            Func<Task> accion = () => _cache.SetAsync("clave", "valor", expiracion);

            //Assert
            await accion.Should().NotThrowAsync();

            _memoryCache.Verify(
                cache => cache.CreateEntry(It.IsAny<object>()),
                Times.Never
            );
        }

        [Test]
        public async Task RemoveAsync_CuandoLaCacheFalla_NoLanzaExcepcion() {
            //Arrange
            _memoryCache
                .Setup(cache => cache.Remove("clave"))
                .Throws(new InvalidOperationException("Cache no disponible"));

            //Act
            Func<Task> accion = () => _cache.RemoveAsync("clave");

            //Assert
            await accion.Should().NotThrowAsync();

            _memoryCache.Verify(
                cache => cache.Remove("clave"),
                Times.Once
            );
        }

        [Test]
        public async Task RemoveAllAsync_ConCacheLiberada_NoPropagaExcepcion() {
            //Arrange
            using var cacheReal = new MicrosoftMemoryCache(new MemoryCacheOptions());
            var cache = new InMemoryCache(cacheReal);

            cacheReal.Dispose();

            //Act
            Func<Task> accion = () => cache.RemoveAllAsync();

            //Assert
            await accion.Should().NotThrowAsync();
        }
    }
}