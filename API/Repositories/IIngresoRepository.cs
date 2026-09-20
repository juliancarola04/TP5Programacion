using API.Models;

using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Ingreso;

namespace API.Repositories
{
    public interface IIngresoRepository
    {
        Task<PaginadoResponse<Ingreso>> ObtenerTodos(IngresoQueryParametros parametros);
        Task<Ingreso?> ObtenerPorId(int id);
        Task<Ingreso?> ObtenerParaAnular(int id);
        Task Crear(Ingreso ingreso);
        Task GuardarCambios();
    }
}
