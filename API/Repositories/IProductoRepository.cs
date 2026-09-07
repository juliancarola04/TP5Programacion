using API.Models;

namespace API.Repositories
{
    public interface IProductoRepository
    {
        Task<List<Producto>> ObtenerTodos();
        Task<Producto?> ObtenerPorId(int id);
        Task<bool> ExistePorNombre(string nombre);
        Task<bool> Existe(int id);
        Task Crear(Producto producto);
        Task Actualizar(Producto producto);
        Task Eliminar(Producto producto);
    }
}
