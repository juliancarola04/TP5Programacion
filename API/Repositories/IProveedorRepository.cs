using API.Models;

namespace API.Repositories;

public interface IProveedorRepository
{
    Task<List<Proveedor>> ObtenerTodos();
    Task<Proveedor?> BuscarPorId(int id);
    Task<bool> ExistePorCuit(string cuit);
    Task<bool> ExistePorEmail(string email);
    Task<bool> ExistePorRazonSocial(string razonSocial);
    Task<bool> ExistePorTelefono(string telefono);
    Task<bool> TieneIngresosAsociados(int proveedorId);
    Task Crear(Proveedor proveedor);
    Task Actualizar(Proveedor proveedor);
    Task DarDeBaja(Proveedor proveedor);
}