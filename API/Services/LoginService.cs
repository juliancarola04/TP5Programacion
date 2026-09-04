using System.Data.Common;
using API.Data;
using API.DTOs;
using API.DTOs.Output;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;

namespace API.Services
{
    public class LoginService
    {
        private readonly ILoginRepository _repo;
        private readonly ITokenService _tokenService;
        
        public LoginService(ILoginRepository repo, ITokenService tokenService)
        {
            _repo = repo;
            _tokenService = tokenService;
        }


        public async Task<LoginDtoOutput?> Login(LoginDtoInput loginDtoInput)
        {
            string username = loginDtoInput.Username.Trim();
            string password = loginDtoInput.Password.Trim();

            if (Validaciones.Requeridos(username, password))
            {
                throw new DatosLlegaronErradosException("Ya sea el usuario o la contraseña llegaron vacíos.");
            }

            try
            {
                Usuario? usuario = await _repo.BuscarPorUsername(username);
                
                if (usuario is null)
                {
                    throw new RecursoNoExisteException("No existe ningún usuario con ese usuario.");
                }

                bool sonIguales = BCrypt.Net.BCrypt.EnhancedVerify(password, usuario.Password);

                if (sonIguales)
                {
                    (string token, DateTime expiracion) = _tokenService.CrearToken(usuario);
                    
                    LoginDtoOutput loginDtoOutput = new LoginDtoOutput()
                    {
                        Exito = true,
                        Token = token,
                        Expiracion = expiracion
                    };

                    return loginDtoOutput;
                }
                else
                {
                    throw new DatosLlegaronErradosException("La contraseña ingresada no coincide.");
                }
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
            
        }

    }
}
