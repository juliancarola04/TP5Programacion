namespace TP5Programacion.Compartidas.DTO.Ingreso.Response;

public record IngresoDetalleResponse(
    int ProductoId,
    string ProductoNombre,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public record IngresoResponse(
    int Id,
    DateTime Fecha,
    decimal Total,
    int ProveedorId,
    string ProveedorRazonSocial,
    int UsuarioId,
    string UsuarioUsername,
    bool Anulado,
    List<IngresoDetalleResponse> Detalles);

public record IngresoListadoResponse(
    int Id,
    DateTime Fecha,
    decimal Total,
    int ProveedorId,
    string ProveedorRazonSocial,
    bool Anulado);