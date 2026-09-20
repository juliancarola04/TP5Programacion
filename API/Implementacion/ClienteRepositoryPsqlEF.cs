using API.Data;
using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Cliente;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion;

public class ClienteRepositoryPsqlEF : IClienteRepository
{
    private readonly DataContext _dataContext;

    public ClienteRepositoryPsqlEF(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PaginadoResponse<Cliente>> ObtenerTodos(ClienteQueryParametros parametros)
    {
        IQueryable<Cliente> query = _dataContext.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parametros.Buscar))
        {
            string buscar = parametros.Buscar.ToLower();
            query = query.Where(c => c.Nombre.ToLower().Contains(buscar) || c.Dni.Contains(buscar) || c.Email.ToLower().Contains(buscar));
        }

        bool descendente = parametros.Direccion?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
        query = parametros.OrdenarPor?.ToLower() switch
        {
            "nombre" => descendente ? query.OrderByDescending(c => c.Nombre).ThenBy(c => c.Id) : query.OrderBy(c => c.Nombre).ThenBy(c => c.Id),
            "email" => descendente ? query.OrderByDescending(c => c.Email).ThenBy(c => c.Id) : query.OrderBy(c => c.Email).ThenBy(c => c.Id),
            _ => descendente ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
        };

        int totalRegistros = await query.CountAsync();
        List<Cliente> clientes = await query
            .Skip((parametros.NumeroPagina - 1) * parametros.TamanoPagina)
            .Take(parametros.TamanoPagina)
            .ToListAsync();

        return new PaginadoResponse<Cliente>(clientes, parametros.NumeroPagina, parametros.TamanoPagina, totalRegistros);
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
