using API.DTOs.Input;
using API.DTOs.Output;
using API.Models;

namespace API.Repositories
{
    public interface IRegisterRepository
    {
        Task Registrarse(Usuario usuario);
    }
}
