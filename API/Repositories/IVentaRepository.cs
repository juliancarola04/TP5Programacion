using API.Models;

namespace API.Repositories
{
    public interface IVentaRepository
    {
        Task<List<Venta>> ObtenerTodas();
        Task<Venta?> ObtenerPorId(int id);
        Task Crear(Venta venta);
    }
}
