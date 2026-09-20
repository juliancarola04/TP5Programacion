using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.UsuarioQuery;

namespace API.Repositories
{
    public interface IUsuarioRepository
    {
        Task<bool> ExistePorUsername(string username);
        Task<bool> ExistePorEmail(string email);
        Task<Usuario?> BuscarPorUsername(string username);
        Task<Usuario?> BuscarPorId(int id);
        Task DarDeBaja(Usuario usuario);
        Task<PaginadoResponse<Usuario>> ObtenerTodos(UsuarioQueryParametros usuarioQueryParametros);
        Task Actualizar(Usuario usuario);
    }
}
