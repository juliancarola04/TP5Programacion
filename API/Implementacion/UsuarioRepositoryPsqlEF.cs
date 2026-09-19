using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion
{
    public class UsuarioRepositoryPsqlEF : IUsuarioRepository
    {
        private readonly DataContext _dataContext;

        public UsuarioRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Usuario?> BuscarPorId(int id)
        {
            return await _dataContext.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Actualizar(Usuario usuario)
        {
            _dataContext.Usuarios.Update(usuario);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DarDeBaja(Usuario usuario)
        {
            _dataContext.Usuarios.Update(usuario);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<Usuario>> ObtenerTodos()
        { 
            return await _dataContext.Usuarios.ToListAsync();;
        }

        public async Task<bool> ExistePorUsername(string username)
        {

            return await _dataContext.Usuarios.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> ExistePorEmail(string email)
        {

            return await _dataContext.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<Usuario?> BuscarPorUsername(string username)
        {
            return await _dataContext.Usuarios.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
