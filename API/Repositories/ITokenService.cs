using API.Models;

namespace API.Repositories
{
    public interface ITokenService
    {
        public (string token, DateTime expiracion) CrearToken(Usuario usuario);
    }
}
