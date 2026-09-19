using API.Data;
using API.Models;
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

    public async Task<List<Ingreso>> ObtenerTodos()
    {
        return await _dataContext.Ingresos
            .Include(i => i.Proveedor)
            .AsNoTracking()
            .OrderByDescending(i => i.Fecha)
            .ToListAsync();
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