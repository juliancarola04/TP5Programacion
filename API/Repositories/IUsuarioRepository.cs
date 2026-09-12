using API.DTOs.Output;
using API.Models;

namespace API.Repositories
{
    public interface IUsuarioRepository
    {
        Task<bool> ExistePorUsername(string username);
        Task<bool> ExistePorEmail(string email);
        Task<Usuario?> BuscarPorUsername(string username);
        Task<Usuario?> BuscarPorId(int id);
        Task DarDeBaja(Usuario usuario);
        Task<List<Usuario>> ObtenerTodos();
        Task Actualizar(Usuario usuario);
    }
}
