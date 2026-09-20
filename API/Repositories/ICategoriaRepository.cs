using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Categoria;

namespace API.Repositories
{
    public interface ICategoriaRepository
    {
        Task<PaginadoResponse<Categoria>> ObtenerTodas(CategoriaQueryParametros parametros);
        Task<Categoria?> ObtenerPorId(int id);
        Task<bool> ExistePorNombre(string nombre);
        Task<bool> TieneProductosAsociados(int categoriaId);

        Task Crear(Categoria categoria);
        Task Actualizar(Categoria categoria);
        Task Eliminar(Categoria categoria);
    }
}
