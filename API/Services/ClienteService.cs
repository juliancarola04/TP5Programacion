using System.Data.Common;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;
using TP5Programacion.Compartidas.DTO.Cliente.Request;
using TP5Programacion.Compartidas.DTO.Cliente.Response;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Cliente;
using TP5Programacion.Compartidas.DTO.Paginado.Request.Cliente;

namespace API.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _repo;

        public ClienteService(IClienteRepository repo)
        {
            _repo = repo;
        }

        public async Task<PaginadoResponse<ClienteResponse>> ObtenerTodos(ParametroPaginacionClienteRequest parametros)
        {
            try
            {
                int numeroPagina = parametros.NumeroPagina is null || parametros.NumeroPagina < 1
                    ? 1
                    : parametros.NumeroPagina.Value;

                int tamanoPagina = parametros.TamanoPagina is null || parametros.TamanoPagina < 1
                    ? 20
                    : parametros.TamanoPagina > 50 ? 50 : parametros.TamanoPagina.Value;

                string? direccion =
                    string.IsNullOrWhiteSpace(parametros.Direccion) &&
                    parametros.Direccion?.ToLower() is not ("asc" or "desc")
                        ? "desc"
                        : parametros.Direccion;

                string? buscar = parametros.Buscar?.Trim().ToLower();

                string? ordenarPor = string.IsNullOrWhiteSpace(parametros.OrdenarPor) ? null : parametros.OrdenarPor;

                ClienteQueryParametros clienteQueryParametros = new ClienteQueryParametros
                {
                    NumeroPagina = numeroPagina,
                    TamanoPagina = tamanoPagina,
                    Buscar = buscar,
                    OrdenarPor = ordenarPor,
                    Direccion = direccion
                };

                PaginadoResponse<Cliente> resultado = await _repo.ObtenerTodos(clienteQueryParametros);

                List<ClienteResponse> clientes = resultado.Datos.Select(c => new ClienteResponse(
                    c.Id, c.Nombre, c.Dni, c.Telefono, c.Email, c.Direccion
                )).ToList();

                return new PaginadoResponse<ClienteResponse>(
                    clientes,
                    resultado.NumeroPagina,
                    resultado.TamanoPagina,
                    resultado.TotalRegistros
                );
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task<ClienteResponse> ObtenerPorId(int id)
        {
            try
            {
                Cliente? cliente = await _repo.ObtenerPorId(id);

                if (cliente is null)
                {
                    throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
                }

                return new ClienteResponse(cliente.Id, cliente.Nombre, cliente.Dni, cliente.Telefono, cliente.Email, cliente.Direccion);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task<ClienteResponse> Crear(CrearClienteRequest dto)
        {
            if (!Validaciones.EstanDatosBien(dto.Nombre, dto.Dni, dto.Telefono, dto.Email, dto.Direccion))
            {
                throw new DatosLlegaronErradosException("Todos los campos del cliente son obligatorios.");
            }

            if (dto.Nombre.Length > 30 || dto.Dni.Length > 8 || dto.Telefono.Length > 13
                || dto.Email.Length > 320 || dto.Direccion.Length > 40)
            {
                throw new DatosLlegaronErradosException("Alguno de los campos excede la longitud máxima permitida.");
            }

            try
            {
                if (await _repo.ExistePorDni(dto.Dni))
                {
                    throw new RecursoExistenteException("Ya existe un cliente con ese DNI.");
                }

                if (await _repo.ExistePorEmail(dto.Email))
                {
                    throw new RecursoExistenteException("Ya existe un cliente con ese email.");
                }

                Cliente cliente = new Cliente
                {
                    Nombre = dto.Nombre,
                    Dni = dto.Dni,
                    Telefono = dto.Telefono,
                    Email = dto.Email,
                    Direccion = dto.Direccion
                };

                await _repo.Crear(cliente);

                return new ClienteResponse(cliente.Id, cliente.Nombre, cliente.Dni, cliente.Telefono, cliente.Email, cliente.Direccion);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarClienteRequest dto)
        {
            if (!Validaciones.EstanDatosBien(dto.Nombre, dto.Dni, dto.Telefono, dto.Email, dto.Direccion))
            {
                throw new DatosLlegaronErradosException("Todos los campos del cliente son obligatorios.");
            }

            try
            {
                Cliente? cliente = await _repo.ObtenerPorId(id);

                if (cliente is null)
                {
                    throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
                }

                // Si cambió el DNI o el Email, hay que re-chequear unicidad (evitar chocar con otro cliente existente)
                if (cliente.Dni != dto.Dni && await _repo.ExistePorDni(dto.Dni))
                {
                    throw new RecursoExistenteException("Ya existe un cliente con ese DNI.");
                }

                if (cliente.Email != dto.Email && await _repo.ExistePorEmail(dto.Email))
                {
                    throw new RecursoExistenteException("Ya existe un cliente con ese email.");
                }

                cliente.Nombre = dto.Nombre;
                cliente.Dni = dto.Dni;
                cliente.Telefono = dto.Telefono;
                cliente.Email = dto.Email;
                cliente.Direccion = dto.Direccion;

                await _repo.Actualizar(cliente);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                Cliente? cliente = await _repo.ObtenerPorId(id);

                if (cliente is null)
                {
                    throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
                }

                if (await _repo.TieneVentasAsociadas(id))
                {
                    throw new RecursoExistenteException("No se puede eliminar el cliente porque tiene ventas asociadas.");
                }

                await _repo.Eliminar(cliente);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
    }
}
