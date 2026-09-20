using System.Data.Common;
using API.Excepciones;
using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Proveedor;
using API.Repositories;
using API.Utilidades;
using TP5Programacion.Compartidas.DTO.Paginado.Request.Usuario;
using TP5Programacion.Compartidas.DTO.Proveedor.Request;
using TP5Programacion.Compartidas.DTO.Proveedor.Response;

namespace API.Services;

public class ProveedorService
{
    private readonly IProveedorRepository _repo;

    public ProveedorService(IProveedorRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<PaginadoResponse<ObtenerProveedorResponse>> ObtenerTodos(ParametroPaginacionProveedorRequest parametros)
    {
        try
        {
            int numeroPagina = parametros.NumeroPagina is null || parametros.NumeroPagina < 1
                ? 1
                : parametros.NumeroPagina.Value;

            int tamanoPagina = parametros.TamanoPagina is null || parametros.TamanoPagina < 1
                ? 20
                : parametros.TamanoPagina > 50 ? 50 : parametros.TamanoPagina.Value;

            bool? eliminado = parametros.Eliminado;

            string? direccion =
                string.IsNullOrWhiteSpace(parametros.Direccion) &&
                parametros.Direccion?.ToLower() is not ("asc" or "desc")
                    ? "desc"
                    : parametros.Direccion;
            
            string? buscar = parametros.Buscar?.Trim().ToLower();
            
            string? ordenarPor = string.IsNullOrWhiteSpace(parametros.OrdenarPor) ? null : parametros.OrdenarPor;
            
            
            ProveedorQueryParametros proveedorQueryParametros = new ProveedorQueryParametros
            {
                NumeroPagina = numeroPagina,
                TamanoPagina = tamanoPagina,
                Eliminado = eliminado,
                Buscar = buscar,
                OrdenarPor = ordenarPor,
                Direccion = direccion
            };
            
            PaginadoResponse<Proveedor> resultado = await _repo.ObtenerTodos(proveedorQueryParametros);

            
            List<ObtenerProveedorResponse> obtenerProveedoresResponse = resultado.Datos.Select(
                p => new ObtenerProveedorResponse
                (
                    p.Id,
                    p.RazonSocial,
                    p.CUIT,
                    p.Direccion,
                    p.Email,
                    p.Telefono
                )).ToList();
                
            return new PaginadoResponse<ObtenerProveedorResponse>(
                obtenerProveedoresResponse,
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

    public async Task<ObtenerProveedorResponse> ObtenerPorId(int id)
    {
        try
        {
            Proveedor? proveedor = await _repo.BuscarPorId(id);

            if (proveedor is null)
            {
                throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
            }

            ObtenerProveedorResponse obtenerProveedorResponse = new ObtenerProveedorResponse
            (
                proveedor.Id,
                proveedor.RazonSocial,
                proveedor.CUIT,
                proveedor.Direccion,
                proveedor.Email,
                proveedor.Telefono
                
            );
            
            return obtenerProveedorResponse;
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task<CrearProveedorResponse> Crear(CrearProveedorRequest dto)
    {
        if (!Validaciones.EstanDatosBien(dto.RazonSocial, dto.Cuit, dto.Direccion, dto.Email, dto.Telefono))
        {
            throw new DatosLlegaronErradosException("Todos los campos del proveedor son obligatorios.");
        }
        
        if (!Validaciones.EsUnEmailValido(dto.Email))
        {
            throw new DatosLlegaronErradosException("El formato del E-Mail es inválido.");
        }

        try
        {
            if (await _repo.ExistePorCuit(dto.Cuit))
            {
                throw new RecursoExistenteException("Ya existe un proveedor con ese CUIT.");
            }

            if (await _repo.ExistePorEmail(dto.Email))
            {
                throw new RecursoExistenteException("Ya existe un proveedor con ese email.");
            }

            Proveedor proveedor = new Proveedor
            {
                RazonSocial = dto.RazonSocial,
                CUIT = dto.Cuit,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion
            };

            await _repo.Crear(proveedor);

            CrearProveedorResponse proveedorDtoOutput = new CrearProveedorResponse
            (
                proveedor.Id,
                proveedor.RazonSocial,
                proveedor.CUIT,
                proveedor.Direccion,
                proveedor.Email,
                proveedor.Telefono
            );

            return proveedorDtoOutput;
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task Actualizar(int id, ActualizarProveedorRequest actualizarProveedorRequest)
    {
        if (Validaciones.EstanDatosBien(id) == false)
        {
            throw new DatosLlegaronErradosException("El ID llegó errado.");
        }

        try
        {
            Proveedor? proveedor = await _repo.BuscarPorId(id);
            bool cambieAlgo = false;

            if (proveedor == null)
            {
                throw new RecursoNoExisteException("No existe ningún usuario con ese ID.");
            }

            if (actualizarProveedorRequest.RazonSocial != proveedor.RazonSocial && Validaciones.EstanDatosBien(actualizarProveedorRequest.RazonSocial))
            {
                if (await _repo.ExistePorRazonSocial(actualizarProveedorRequest.RazonSocial!))
                {
                    throw new RecursoExistenteException("Ya existe alguien con esa razón social.");
                }

                cambieAlgo = true;
                proveedor.RazonSocial = actualizarProveedorRequest.RazonSocial!;
            }

            if (actualizarProveedorRequest.Cuit != proveedor.CUIT && Validaciones.EstanDatosBien(actualizarProveedorRequest.Cuit))
            {
                if (await _repo.ExistePorRazonSocial(actualizarProveedorRequest.Cuit!))
                {
                    throw new RecursoExistenteException("Ya existe un proveedor con ese CUIT.");
                }
                
                cambieAlgo = true;
                proveedor.CUIT = actualizarProveedorRequest.Cuit!;
                
            }

            if (actualizarProveedorRequest.Direccion != proveedor.Direccion && Validaciones.EstanDatosBien(proveedor.Direccion))
            {
                cambieAlgo = true;
                proveedor.Direccion = actualizarProveedorRequest.Direccion!;
            }

            if (actualizarProveedorRequest.Email != proveedor.Email && Validaciones.EstanDatosBien(actualizarProveedorRequest.Email))
            {
                if (!Validaciones.EsUnEmailValido(actualizarProveedorRequest.Email!))
                {
                    throw new DatosLlegaronErradosException("El formato del E-Mail es inválido.");
                }

                if (await _repo.ExistePorEmail(actualizarProveedorRequest.Email!))
                {
                    throw new RecursoExistenteException("Ya existe un proveedor con ese E-Mail.");
                }

                cambieAlgo = true;
                proveedor.Email = actualizarProveedorRequest.Email!;
            }

            if (actualizarProveedorRequest.Telefono != proveedor.Telefono && Validaciones.EstanDatosBien(actualizarProveedorRequest.Telefono))
            {
                if (await _repo.ExistePorRazonSocial(actualizarProveedorRequest.Cuit!))
                {
                    throw new RecursoExistenteException("Ya existe un proveedor con ese CUIT.");
                }
                
                cambieAlgo = true;
                proveedor.Telefono = actualizarProveedorRequest.Telefono!;
                    
            }

            if (cambieAlgo)
            {
                await _repo.Actualizar(proveedor);
            }
            else
            {
                throw new DatosLlegaronErradosException("Los datos que mandó fueron inválidos");
            }
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Pasó un problema y no se pudo actualizar el usuario: {e.Message}");
        }
    }

    public async Task DarDeBaja(int id)
    {
        if (Validaciones.EstanDatosBien(id) == false)
        {
            throw new DatosLlegaronErradosException("El ID llegó errado.");
        }

        try
        {
            Proveedor? proveedor = await _repo.BuscarPorId(id);

            if (proveedor == null)
            {
                throw new RecursoNoExisteException("No existe ningún usuario con ese ID.");
            }

            proveedor.Eliminado = true;

            await _repo.DarDeBaja(proveedor);
        }
        catch (DbException)
        {
            throw new BaseDeDatosException("Ocurrió un problema a la hora de contactar con la base de datos.");
        }
    }
}