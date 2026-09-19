namespace TP5Programacion.Compartidas.DTO.Ingreso.Request;

public record CrearIngresoItemRequest(int ProductoId, int Cantidad, decimal PrecioUnitario);

public record CrearIngresoRequest(
    int ProveedorId,
    List<CrearIngresoItemRequest> Items);