using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using InMemoryCache = RepositorioRemoto.Back.Cache.MemoryCache;

namespace RepositorioRemoto.Tests.Cache;

public abstract class MemoryCacheTests
{
    [TestFixture]
    public class CasosValidos
    {
        private Mock<IMemoryCache> _memoryCache = null!;
        private InMemoryCache _cache = null!;

        [SetUp]
        public void SetUp()
        {
            _memoryCache = new Mock<IMemoryCache>();
            _cache = new InMemoryCache(_memoryCache.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache.Reset();
        }

        [Test]
        public async Task GetAsync_DebeRetornarElValorGuardado()
        {
            var usuario = new UsuarioCache(1, "Laura");
            object? valorGuardado = usuario;

            _memoryCache
                .Setup(cache => cache.TryGetValue("usuario:1", out valorGuardado))
                .Returns(true);

            var result = await _cache.GetAsync<UsuarioCache>("usuario:1");

            result.Should().BeEquivalentTo(usuario);
        }

        [Test]
        public async Task SetAsync_DebeGuardarElValorYConfigurarLaExpiracion()
        {
            var entry = new Mock<ICacheEntry>();
            var usuario = new UsuarioCache(1, "Laura");
            var expiration = TimeSpan.FromMinutes(2);

            _memoryCache
                .Setup(cache => cache.CreateEntry("usuario:1"))
                .Returns(entry.Object);

            await _cache.SetAsync("usuario:1", usuario, expiration);

            _memoryCache.Verify(
                cache => cache.CreateEntry("usuario:1"),
                Times.Once);
            entry.VerifySet(value => value.Value = usuario, Times.Once);
            entry.VerifySet(
                value => value.AbsoluteExpirationRelativeToNow = expiration,
                Times.Once);
        }

        [Test]
        public async Task RemoveAsync_DebeEliminarLaClave()
        {
            await _cache.RemoveAsync("clave");

            _memoryCache.Verify(
                cache => cache.Remove("clave"),
                Times.Once);
        }

        [Test]
        public async Task RemoveAllAsync_DebeCompletarSinLanzarExcepcion()
        {
            var act = () => _cache.RemoveAllAsync();

            await act.Should().NotThrowAsync();
            _memoryCache.VerifyNoOtherCalls();
        }

        private sealed record UsuarioCache(int Id, string Nombre);
    }

    [TestFixture]
    public class CasosInvalidosYExcepciones
    {
        private Mock<IMemoryCache> _memoryCache = null!;
        private InMemoryCache _cache = null!;

        [SetUp]
        public void SetUp()
        {
            _memoryCache = new Mock<IMemoryCache>();
            _cache = new InMemoryCache(_memoryCache.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache.Reset();
        }

        [Test]
        public async Task SetAsync_DebeCompletarSinLanzarExcepcion_CuandoLaCacheFalla()
        {
            _memoryCache
                .Setup(cache => cache.CreateEntry("clave"))
                .Throws(new InvalidOperationException("Cache no disponible"));

            var act = () => _cache.SetAsync("clave", "valor");

            await act.Should().NotThrowAsync();
        }

        [Test]
        public async Task RemoveAsync_DebeCompletarSinLanzarExcepcion_CuandoLaCacheFalla()
        {
            _memoryCache
                .Setup(cache => cache.Remove("clave"))
                .Throws(new InvalidOperationException("Cache no disponible"));

            var act = () => _cache.RemoveAsync("clave");

            await act.Should().NotThrowAsync();
        }
    }
}
