using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Cliente;

namespace API.Repositories
{
    public interface IClienteRepository
    {
        Task<PaginadoResponse<Cliente>> ObtenerTodos(ClienteQueryParametros parametros);
        Task<Cliente?> ObtenerPorId(int id);
        Task<bool> ExistePorDni(string dni);
        Task<bool> ExistePorEmail(string email);
        Task<bool> TieneVentasAsociadas(int clienteId);

        Task Crear(Cliente cliente);
        Task Actualizar(Cliente cliente);
        Task Eliminar(Cliente cliente);
    }
}
