using API.Data;
using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Ingreso;
using API.Repositories;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion;

public class IngresoRepositoryPsqlEF : IIngresoRepository
{
    private readonly DataContext _dataContext;

    public IngresoRepositoryPsqlEF(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PaginadoResponse<Ingreso>> ObtenerTodos(IngresoQueryParametros parametros)
    {
        IQueryable<Ingreso> query = _dataContext.Ingresos
            .Include(i => i.Proveedor)
            .AsNoTracking();

        if (parametros.ProveedorId.HasValue)
            query = query.Where(i => i.ProveedorId == parametros.ProveedorId.Value);

        if (parametros.Anulado.HasValue)
            query = query.Where(i => i.Anulado == parametros.Anulado.Value);

        if (!string.IsNullOrWhiteSpace(parametros.Buscar))
        {
            string buscar = parametros.Buscar.ToLower();
            query = query.Where(i => i.Proveedor.RazonSocial.ToLower().Contains(buscar));
        }

        bool descendente = parametros.Direccion?.Equals("desc", StringComparison.OrdinalIgnoreCase) != false;
        query = parametros.OrdenarPor?.ToLower() switch
        {
            "total" => descendente ? query.OrderByDescending(i => i.Total).ThenByDescending(i => i.Id) : query.OrderBy(i => i.Total).ThenBy(i => i.Id),
            "fecha" => descendente ? query.OrderByDescending(i => i.Fecha).ThenByDescending(i => i.Id) : query.OrderBy(i => i.Fecha).ThenBy(i => i.Id),
            _ => descendente ? query.OrderByDescending(i => i.Fecha).ThenByDescending(i => i.Id) : query.OrderBy(i => i.Fecha).ThenBy(i => i.Id)
        };

        int totalRegistros = await query.CountAsync();
        List<Ingreso> ingresos = await query
            .Skip((parametros.NumeroPagina - 1) * parametros.TamanoPagina)
            .Take(parametros.TamanoPagina)
            .ToListAsync();

        return new PaginadoResponse<Ingreso>(ingresos, parametros.NumeroPagina, parametros.TamanoPagina, totalRegistros);
    }

    public async Task<Ingreso?> ObtenerPorId(int id)
    {
        return await _dataContext.Ingresos
            .Include(i => i.Proveedor)
            .Include(i => i.Usuario)
            .Include(i => i.DetallesIngresos)
                .ThenInclude(d => d.Producto)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    // Trackeada: la vamos a modificar (Anulado + Stock de cada Producto).
    public async Task<Ingreso?> ObtenerParaAnular(int id)
    {
        return await _dataContext.Ingresos
            .Include(i => i.DetallesIngresos)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task Crear(Ingreso ingreso)
    {
        _dataContext.Ingresos.Add(ingreso);
        await _dataContext.SaveChangesAsync();
    }

    public async Task GuardarCambios()
    {
        await _dataContext.SaveChangesAsync();
    }
}
