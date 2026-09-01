using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Configuraciones
{
    public class DetalleIngresoConfiguracion : IEntityTypeConfiguration<DetalleIngreso>
    {
        public void Configure(EntityTypeBuilder<DetalleIngreso> builder)
        {
            builder.ToTable("DetallesIngresos");

            builder.HasKey(x => new { x.ProductoId, x.IngresoId });

            builder.Property(x => x.Cantidad)
                .IsRequired();

            builder.Property(x => x.PrecioUnitario)
                .IsRequired();

            builder.HasOne(x => x.Producto)
                .WithMany(x => x.DetallesIngresos)
                .HasForeignKey(x => x.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Ingreso)
                .WithMany(x => x.DetallesIngresos)
                .HasForeignKey(x => x.IngresoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
