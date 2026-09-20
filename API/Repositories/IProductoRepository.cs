using API.Models;

using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Producto;

namespace API.Repositories
{
    public interface IProductoRepository
    {
        Task<PaginadoResponse<Producto>> ObtenerTodos(ProductoQueryParametros parametros);
        Task<Producto?> ObtenerPorId(int id);
        Task<bool> ExistePorNombre(string nombre);
        Task<bool> Existe(int id);
        Task Crear(Producto producto);
        Task Actualizar(Producto producto);
        Task Eliminar(Producto producto);
    }
}
