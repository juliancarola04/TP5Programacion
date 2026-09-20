using System.Data.Common;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;
using TP5Programacion.Compartidas.DTO.Auth.Response;
using TP5Programacion.Compartidas.DTO.Auth.Request;
namespace API.Services
{
    public class RegisterService
    {
        private readonly IRegisterRepository _repo;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;


        public RegisterService(IRegisterRepository repo, IUsuarioRepository usuarioRepository, ITokenService tokenService)
        {
            _repo = repo;
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        public async Task<RegisterResponse?> Registrarse(RegisterRequest registerRequest)
        {
            string username = registerRequest.Username.Trim();
            string password = registerRequest.Password.Trim();
            string email = registerRequest.Email.Trim();

            if (Validaciones.EstanDatosBien(username, password, email) == false)
            {
                throw new DatosLlegaronErradosException("Alguno de los datos llegó vacío.");
            }
            
            if (!Validaciones.EsUnEmailValido(email))
            {
                throw new DatosLlegaronErradosException("El formato del E-Mail es inválido.");
            }

            try
            {
                if (await _usuarioRepository.ExistePorUsername(username))
                {
                    throw new RecursoExistenteException("Ya existe alguien con ese usuario.");
                }
                
                if (await _usuarioRepository.ExistePorEmail(email))
                {
                    throw new RecursoExistenteException("Ya existe alguien con ese email.");
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
                Email = email,
            };

            try
            {
                await _repo.Registrarse(usuario);

                (string token, DateTime expiracion) = _tokenService.CrearToken(usuario);

                RegisterResponse registerDtoOutput = new RegisterResponse(token);

                return registerDtoOutput;
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Pasó un problema y no se pudo crear el usuario: {e.Message}");
            }

        }
    }
}
