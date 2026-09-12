using API.Models;
namespace API.Repositories
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> ObtenerTodos();
        Task<Cliente?> ObtenerPorId(int id);
        Task<bool> ExistePorDni(string dni);
        Task<bool> ExistePorEmail(string email);
        Task<bool> TieneVentasAsociadas(int clienteId);

        Task Crear(Cliente cliente);
        Task Actualizar(Cliente cliente);
        Task Eliminar(Cliente cliente);
    }
}
