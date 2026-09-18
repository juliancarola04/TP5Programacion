using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion
{
    public class VentaRepositoryPostgreSQL : IVentaRepository
    {
    private readonly DataContext _dataContext;
    public VentaRepositoryPostgreSQL(DataContext dataContext)
    {
        _dataContext = dataContext;

    }
    public async Task<List<Venta>> ObtenerTodas()
    {
        return await _dataContext.Ventas
            .Include(v => v.Cliente)
            .AsNoTracking()
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();
    }
    public async Task<Venta?> ObtenerPorId(int id)
    {
        return await _dataContext.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.DetallesVentas)
                .ThenInclude(d => d.Producto)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

    }
        public async Task<Venta?> ObtenerParaAnular(int id)
        {
            return await _dataContext.Ventas
                .Include(v => v.DetallesVentas)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
        public async Task Crear(Venta venta)
        {
            _dataContext.Ventas.Add(venta);
            await _dataContext.SaveChangesAsync();
        }
        public async Task GuardarCambios()
        {
            await _dataContext.SaveChangesAsync();
        }

    }

}
