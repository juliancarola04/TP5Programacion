using API.Models;

using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Venta;

namespace API.Repositories
{
    public interface IVentaRepository
    {
        Task<PaginadoResponse<Venta>> ObtenerTodas(VentaQueryParametros parametros);
        Task<Venta?> ObtenerPorId(int id);
        Task<Venta?> ObtenerParaAnular(int id);
        Task Crear(Venta venta);
        Task GuardarCambios();
    }
}
