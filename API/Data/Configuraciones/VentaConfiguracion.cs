using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Configuraciones
{
    public class VentaConfiguracion : IEntityTypeConfiguration<Venta>
    {
        public void Configure(EntityTypeBuilder<Venta> builder)
        {
            builder.ToTable("Ventas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Total)
                .IsRequired();

            builder.Property(x => x.Fecha)
                .IsRequired();

            builder.HasOne(x => x.Usuario)
                .WithMany(x => x.Ventas)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cliente)
                .WithMany(x => x.Ventas)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.UsuarioId);
            builder.HasIndex(x => x.ClienteId);
        }
    }
}
