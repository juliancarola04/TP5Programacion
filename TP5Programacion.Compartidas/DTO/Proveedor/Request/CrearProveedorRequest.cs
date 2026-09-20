namespace TP5Programacion.Compartidas.DTO.Proveedor.Request;

public record CrearProveedorRequest(
    string RazonSocial,
    string Cuit,
    string Direccion,
    string Email,
    string Telefono);