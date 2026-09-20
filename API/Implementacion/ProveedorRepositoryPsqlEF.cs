using API.Data;
using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Proveedor;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion;

public class ProveedorRepositoryPsqlEF : IProveedorRepository
{
    private readonly DataContext _dataContext;

    public ProveedorRepositoryPsqlEF(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<PaginadoResponse<Proveedor>> ObtenerTodos(ProveedorQueryParametros proveedorQueryParametros)
    {
        IQueryable<Proveedor> query = _dataContext.Proveedores.AsNoTracking().AsQueryable();

        if (proveedorQueryParametros.Eliminado.HasValue)
        {
            query = query
                .IgnoreQueryFilters()
                .Where(p => p.Eliminado == proveedorQueryParametros.Eliminado.Value);

        }

        // chequeamos si quiere que sea descendiente
        bool descendente = proveedorQueryParametros.Direccion?.Equals("desc", StringComparison.OrdinalIgnoreCase) ==
                            true;
        if (!string.IsNullOrWhiteSpace(proveedorQueryParametros.Buscar))
        {
            query = query.Where(p => p.RazonSocial.ToLower().Contains(proveedorQueryParametros.Buscar));
        }

        query = proveedorQueryParametros.OrdenarPor?.ToLower() switch
        {
            "razonsocial" => descendente
                ? query.OrderByDescending(p => p.RazonSocial).ThenBy(p => p.Id)
                : query.OrderBy(p => p.RazonSocial).ThenBy(p => p.Id),
            _ => query.OrderBy(p => p.Id) // Se va a ordenar por defecto por el ID.
        };
        
        int totalRegistros = await query.CountAsync();
        
        List<Proveedor> proveedores = await query
            .Skip((proveedorQueryParametros.NumeroPagina - 1) * proveedorQueryParametros.TamanoPagina)
            .Take(proveedorQueryParametros.TamanoPagina).ToListAsync();
        
        return new PaginadoResponse<Proveedor>(proveedores, proveedorQueryParametros.NumeroPagina, proveedorQueryParametros.TamanoPagina, totalRegistros);
    }

    public async Task<Proveedor?> BuscarPorId(int id)
    {
        return await _dataContext.Proveedores.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExistePorCuit(string cuit)
    {
        return await _dataContext.Proveedores.AnyAsync(p => p.CUIT == cuit);
        
    }

    public async Task<bool> ExistePorEmail(string email)
    {
        return await _dataContext.Proveedores.AnyAsync(p => p.Email == email);
    }
    public async Task<bool> ExistePorRazonSocial(string razonSocial)
    {
        return await _dataContext.Proveedores.AnyAsync(p => p.RazonSocial == razonSocial);
    }

    public async Task<bool> ExistePorTelefono(string telefono)
    {
        return await _dataContext.Proveedores.AnyAsync(p => p.Telefono == telefono);
    }

    public async Task<bool> TieneIngresosAsociados(int proveedorId)
    {
        return await _dataContext.Ingresos.AnyAsync(i => i.ProveedorId == proveedorId);
    }

    public async Task Crear(Proveedor proveedor)
    {
        _dataContext.Proveedores.Add(proveedor);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task Actualizar(Proveedor proveedor)
    {
        _dataContext.Proveedores.Update(proveedor);
        await _dataContext.SaveChangesAsync();
    }

    public async Task DarDeBaja(Proveedor proveedor)
    {
        _dataContext.Proveedores.Update(proveedor);
        await _dataContext.SaveChangesAsync();
    }

}
