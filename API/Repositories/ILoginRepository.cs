using API.DTOs;
using API.DTOs.Output;
using API.Models;

namespace API.Repositories;

public interface ILoginRepository
{ 
    
    Task<bool> ExistePorUsername(string username);
    Task<bool> ExistePorEmail(string email);

    Task<Usuario?> BuscarPorUsername(string username);
}