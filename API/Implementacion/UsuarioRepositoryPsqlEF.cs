using API.Data;
using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.UsuarioQuery;
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

        public async Task<PaginadoResponse<Usuario>> ObtenerTodos(UsuarioQueryParametros usuarioQueryParametros)
        {
            // Le vamos a ir escribiendo al query cositas que desp se van a reflejar en una query de verdad. Pero no la sobreescribimos cuando la volvemos a asignar, simplemente le agregmaos otro where.
            IQueryable<Usuario> query = _dataContext.Usuarios.AsNoTracking().AsQueryable();

            if (usuarioQueryParametros.EsAdministrador.HasValue)
            {
                query = query.Where(u => u.EsAdministrador == usuarioQueryParametros.EsAdministrador);
            }
            
            if (usuarioQueryParametros.Eliminado.HasValue)
            {
                query = query
                    .IgnoreQueryFilters()
                    .Where(u => u.Eliminado == usuarioQueryParametros.Eliminado.Value);
            }

            int totalRegistros = await query.CountAsync();

            // Esto chusmealo del PDF que subió Trani que está relativamente bien explicado.
            List<Usuario> usuarios = await query
                .Skip((usuarioQueryParametros.NumeroPagina - 1) * usuarioQueryParametros.TamanoPagina)
                .Take(usuarioQueryParametros.TamanoPagina).ToListAsync();
            
            return new PaginadoResponse<Usuario>(usuarios, usuarioQueryParametros.NumeroPagina, usuarioQueryParametros.TamanoPagina, totalRegistros);
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
