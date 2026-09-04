using System.Data.Common;
using System.Linq.Expressions;
using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RegisterService
    {
        private readonly IRegisterRepository _repo;
        private readonly ILoginRepository _loginRepo;
        private readonly ITokenService _tokenService;


        public RegisterService(IRegisterRepository repo, ILoginRepository loginRepo, ITokenService tokenService)
        {
            _repo = repo;
            _loginRepo = loginRepo;
            _tokenService = tokenService;
        }

        public async Task<RegisterDtoOutput?> Registrarse(RegisterDtoInput registerDtoInput)
        {
            string username = registerDtoInput.Username.Trim();
            string password = registerDtoInput.Password.Trim();
            string email = registerDtoInput.Email.Trim();

            if (Validaciones.Requeridos(username, password, email))
            {
                throw new DatosLlegaronErradosException("Alguno de los datos llegó vacío.");
            }

            try
            {
                if (await _loginRepo.ExistePorUsername(username))
                {
                    throw new RecursoExistenteException("Ya existe alguien con ese usuario.");
                }
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
            
            Usuario usuario = new Usuario()
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password),
                Email = email
            };

            try
            {
                await _repo.Registrarse(usuario);

                (string token, DateTime expiracion) = _tokenService.CrearToken(usuario);

                RegisterDtoOutput registerDtoOutput = new RegisterDtoOutput()
                {
                    Exito = true,
                    Token = token,
                    Expiracion = expiracion
                };

                return registerDtoOutput;
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Pasó un problema y no se pudo crear el usuario: {e.Message}");
            }
            
            


        }
    }
}
