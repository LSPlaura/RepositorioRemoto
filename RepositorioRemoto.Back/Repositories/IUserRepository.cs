using RepositorioRemoto.Back.Models;
using RepositorioRemoto.Back.Repositories.Common;

namespace RepositorioRemoto.Back.Repositories;

public interface IUserRepository : ICrudRepositoryAsync<int, User> { }