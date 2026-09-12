using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion;

public class ProveedorRepositoryPostgreSQL : IProveedorRepository
{
    private readonly DataContext _dataContext;

    public ProveedorRepositoryPostgreSQL(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<List<Proveedor>> ObtenerTodos()
    {
        return await _dataContext.Proveedores.ToListAsync();
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