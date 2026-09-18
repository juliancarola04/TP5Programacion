using System.Globalization;

namespace API.DTOs.Output
{
    public record DetalleVentaDtoOutput(
        int ProductoId,
        string ProductoNombre,
        int Cantidad,
        decimal PrecioUnitario,
        decimal Subtotal);
    public record VentaDtoOutput(
        int Id,
        DateTime Fecha,
        decimal Total,
        int ClienteId,
        string ClienteNombre,
        int UsuarioId,
        string UsuarioUsername,
        bool Anulada,
        List<DetalleVentaDtoOutput> Detalles);

    public record VentaListadoDtoOutput(
        int Id,
        DateTime Fecha,
        decimal Total,
        int ClienteId,
        string ClienteNombre,
        bool Anulada);
}
