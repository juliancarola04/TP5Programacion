using API.Models;

namespace API.Repositories
{
    public interface IIngresoRepository
    {
        Task<List<Ingreso>> ObtenerTodos();
        Task<Ingreso?> ObtenerPorId(int id);
        Task<Ingreso?> ObtenerParaAnular(int id);
        Task Crear(Ingreso ingreso);
        Task GuardarCambios();
    }
}
