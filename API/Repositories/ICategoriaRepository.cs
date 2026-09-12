using API.Models;
namespace API.Repositories
{
    public interface ICategoriaRepository
    {
        Task<List<Categoria>> ObtenerTodas();
        Task<Categoria?> ObtenerPorId(int id);
        Task<bool> ExistePorNombre(string nombre);
        Task<bool> TieneProductosAsociados(int categoriaId);

        Task Crear(Categoria categoria);
        Task Actualizar(Categoria categoria);
        Task Eliminar(Categoria categoria);
    }
}
