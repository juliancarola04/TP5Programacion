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
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        
        public LoginService(ILoginRepository repo, ITokenService tokenService, IUsuarioRepository usuarioRepository)
        {
            _repo = repo;
            _tokenService = tokenService;
            _usuarioRepository = usuarioRepository;
        }


        public async Task<LoginDtoOutput?> Login(LoginDtoInput loginDtoInput)
        {
            string username = loginDtoInput.Username;
            string password = loginDtoInput.Password;

            if (Validaciones.EstanDatosBien(username, password) == false)
            {
                throw new DatosLlegaronErradosException("Ya sea el usuario o la contraseña llegaron vacíos.");
            }

            try
            {
                Usuario? usuario = await _usuarioRepository.BuscarPorUsername(username);
                
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
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}"); // Quizá acá habría que quitar el e.message.
            }
            
        }

    }
}
