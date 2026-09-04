using API.Data;
using API.DTOs;
using API.DTOs.Output;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion;

public class LoginRepositoryPostgreSQL : ILoginRepository
{
    private readonly DataContext _dataContext;

    public LoginRepositoryPostgreSQL(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<bool> ExistePorUsername(string username)
    {
        
        return await _dataContext.Usuarios.AnyAsync(u=> u.Username == username);
    }

    public async Task<Usuario?> BuscarPorUsername(string username)
    {
        return await _dataContext.Usuarios.FirstOrDefaultAsync(u => u.Username == username);
    }
}