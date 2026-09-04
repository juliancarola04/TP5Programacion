using API.Data;
using API.DTOs;
using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Repositories;
using API.Models;
using BCrypt;

namespace API.Implementacion
{
    public class RegisterRepositoryPostgreSQL : IRegisterRepository
    {
        private readonly DataContext _dataContext;
        public RegisterRepositoryPostgreSQL(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public async Task Registrarse (Usuario usuario)
        {
            _dataContext.Usuarios.Add(usuario);

            await _dataContext.SaveChangesAsync();
        }
    }
}
