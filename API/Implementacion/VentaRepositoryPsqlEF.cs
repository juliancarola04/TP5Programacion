using API.Data;
using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Venta;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion
{
    public class VentaRepositoryPsqlEF : IVentaRepository
    {
    private readonly DataContext _dataContext;
    public VentaRepositoryPsqlEF(DataContext dataContext)
    {
        _dataContext = dataContext;

    }
    public async Task<PaginadoResponse<Venta>> ObtenerTodas(VentaQueryParametros parametros)
    {
        IQueryable<Venta> query = _dataContext.Ventas
            .Include(v => v.Cliente)
            .AsNoTracking();

        if (parametros.ClienteId.HasValue)
            query = query.Where(v => v.ClienteId == parametros.ClienteId.Value);

        if (parametros.Anulada.HasValue)
            query = query.Where(v => v.Anulada == parametros.Anulada.Value);

        if (!string.IsNullOrWhiteSpace(parametros.Buscar))
        {
            string buscar = parametros.Buscar.ToLower();
            query = query.Where(v => v.Cliente.Nombre.ToLower().Contains(buscar));
        }

        bool descendente = parametros.Direccion?.Equals("desc", StringComparison.OrdinalIgnoreCase) != false;
        query = parametros.OrdenarPor?.ToLower() switch
        {
            "total" => descendente ? query.OrderByDescending(v => v.Total).ThenByDescending(v => v.Id) : query.OrderBy(v => v.Total).ThenBy(v => v.Id),
            "fecha" => descendente ? query.OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id) : query.OrderBy(v => v.Fecha).ThenBy(v => v.Id),
            _ => descendente ? query.OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id) : query.OrderBy(v => v.Fecha).ThenBy(v => v.Id)
        };

        int totalRegistros = await query.CountAsync();
        List<Venta> ventas = await query
            .Skip((parametros.NumeroPagina - 1) * parametros.TamanoPagina)
            .Take(parametros.TamanoPagina)
            .ToListAsync();

        return new PaginadoResponse<Venta>(ventas, parametros.NumeroPagina, parametros.TamanoPagina, totalRegistros);
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
