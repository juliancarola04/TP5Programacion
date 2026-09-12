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
}