using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion;

public class ClienteRepositoryPostgreSQL : IClienteRepository
{
    private readonly DataContext _dataContext;

    public ClienteRepositoryPostgreSQL(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<Cliente>> ObtenerTodos()
    {
        return await _dataContext.Clientes
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorId(int id)
    {
        return await _dataContext.Clientes
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExistePorDni(string dni)
    {
        return await _dataContext.Clientes.AnyAsync(c => c.Dni == dni);
    }

    public async Task<bool> ExistePorEmail(string email)
    {
        return await _dataContext.Clientes.AnyAsync(c => c.Email == email);
    }

    public async Task<bool> TieneVentasAsociadas(int clienteId)
    {
        return await _dataContext.Ventas.AnyAsync(v => v.ClienteId == clienteId);
    }

    public async Task Crear(Cliente cliente)
    {
        _dataContext.Clientes.Add(cliente);
        await _dataContext.SaveChangesAsync();
    }

    public async Task Actualizar(Cliente cliente)
    {
        _dataContext.Clientes.Update(cliente);
        await _dataContext.SaveChangesAsync();
    }

    public async Task Eliminar(Cliente cliente)
    {
        _dataContext.Clientes.Remove(cliente);
        await _dataContext.SaveChangesAsync();
    }
}
