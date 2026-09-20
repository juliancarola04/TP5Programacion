using API.Data;
using API.Excepciones;
using API.Repositories;
using API.Models;
using BCrypt;

namespace API.Implementacion
{
    public class RegisterRepositoryPsqlEF : IRegisterRepository
    {
        private readonly DataContext _dataContext;
        public RegisterRepositoryPsqlEF(DataContext dataContext)
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
