namespace TP5Programacion.Compartidas.DTO.Proveedor.Request;

public record ActualizarProveedorRequest(
    string RazonSocial,
    string Cuit,
    string Direccion,
    string Email,
    string Telefono);