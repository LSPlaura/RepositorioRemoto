using FluentAssertions;
using Moq;
using RepositorioRemoto.Back.Cache;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace RepositorioRemoto.Tests.Cache;

public abstract class RedisCacheTests
{
    [TestFixture]
    public class CasosValidos
    {
        private readonly RedisContainer _redisContainer = new RedisBuilder("redis:7-alpine")
            .Build();

        private IConnectionMultiplexer _redis = null!;
        private RedisCache _cache = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await _redisContainer.StartAsync();
            var configuration = ConfigurationOptions.Parse(_redisContainer.GetConnectionString());
            configuration.AllowAdmin = true;
            _redis = await ConnectionMultiplexer.ConnectAsync(configuration);
            _cache = new RedisCache(_redis);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _redis.DisposeAsync();
            await _redisContainer.DisposeAsync();
        }

        [SetUp]
        public async Task SetUp()
        {
            await _cache.RemoveAllAsync();
        }

        [Test]
        public async Task GetAsync_DebeRetornarDefault_CuandoLaClaveNoExiste()
        {
            var result = await _cache.GetAsync<string>("clave-inexistente");

            result.Should().BeNull();
        }

        [Test]
        public async Task SetAsyncYGetAsync_DebenGuardarYRecuperarUnValor()
        {
            await _cache.SetAsync("usuario:1", new UsuarioCache(1, "Laura"));

            var result = await _cache.GetAsync<UsuarioCache>("usuario:1");

            result.Should().BeEquivalentTo(new UsuarioCache(1, "Laura"));
        }

        [Test]
        public async Task SetAsync_DebeGuardarElValorConLaExpiracionIndicada()
        {
            await _cache.SetAsync("clave-con-ttl", "valor", TimeSpan.FromMinutes(2));

            var ttl = await _redis.GetDatabase().KeyTimeToLiveAsync("clave-con-ttl");

            ttl.Should().NotBeNull();
            ttl.Should().BeLessThanOrEqualTo(TimeSpan.FromMinutes(2));
            ttl.Should().BeGreaterThan(TimeSpan.Zero);
        }

        [Test]
        public async Task SetAsync_DebeUsarCincoMinutosComoExpiracionPorDefecto()
        {
            await _cache.SetAsync("clave-ttl-por-defecto", "valor");

            var ttl = await _redis.GetDatabase().KeyTimeToLiveAsync("clave-ttl-por-defecto");

            ttl.Should().NotBeNull();
            ttl.Should().BeLessThanOrEqualTo(TimeSpan.FromMinutes(5));
            ttl.Should().BeGreaterThan(TimeSpan.Zero);
        }

        [Test]
        public async Task RemoveAsync_DebeEliminarLaClave()
        {
            await _cache.SetAsync("clave-a-eliminar", "valor");

            await _cache.RemoveAsync("clave-a-eliminar");

            var result = await _cache.GetAsync<string>("clave-a-eliminar");
            result.Should().BeNull();
        }

        [Test]
        public async Task RemoveAllAsync_DebeEliminarTodasLasClaves()
        {
            await _cache.SetAsync("clave:1", "valor 1");
            await _cache.SetAsync("clave:2", "valor 2");

            await _cache.RemoveAllAsync();

            (await _cache.GetAsync<string>("clave:1")).Should().BeNull();
            (await _cache.GetAsync<string>("clave:2")).Should().BeNull();
        }

        [Test]
        public async Task GetAsync_DebeRetornarDefault_CuandoElValorNoEsJsonValido()
        {
            await _redis.GetDatabase().StringSetAsync("json-invalido", "{no es json}");

            var result = await _cache.GetAsync<UsuarioCache>("json-invalido");

            result.Should().BeNull();
        }

        private sealed record UsuarioCache(int Id, string Nombre);
    }

    [TestFixture]
    public class CasosInvalidosYExcepciones
    {
        private Mock<IConnectionMultiplexer> _redis = null!;
        private RedisCache _cache = null!;

        [SetUp]
        public void SetUp()
        {
            _redis = new Mock<IConnectionMultiplexer>();
            _cache = new RedisCache(_redis.Object);
        }

        [Test]
        public async Task GetAsync_DebeRetornarDefault_CuandoRedisLanzaUnaExcepcion()
        {
            _redis
                .Setup(redis => redis.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Throws(new RedisException("Redis no disponible"));

            var result = await _cache.GetAsync<string>("clave");

            result.Should().BeNull();
        }

        [Test]
        public async Task SetAsync_DebeCompletarSinLanzarExcepcion_CuandoRedisFalla()
        {
            _redis
                .Setup(redis => redis.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Throws(new RedisException("Redis no disponible"));

            var act = () => _cache.SetAsync("clave", "valor");

            await act.Should().NotThrowAsync();
        }

        [Test]
        public async Task RemoveAsync_DebeCompletarSinLanzarExcepcion_CuandoRedisFalla()
        {
            _redis
                .Setup(redis => redis.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Throws(new RedisException("Redis no disponible"));

            var act = () => _cache.RemoveAsync("clave");

            await act.Should().NotThrowAsync();
        }

        [Test]
        public async Task RemoveAllAsync_DebeCompletarSinLanzarExcepcion_CuandoRedisFalla()
        {
            _redis
                .Setup(redis => redis.GetEndPoints(It.IsAny<bool>()))
                .Throws(new RedisException("Redis no disponible"));

            var act = () => _cache.RemoveAllAsync();

            await act.Should().NotThrowAsync();
        }
    }
}
