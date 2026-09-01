using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Configuraciones
{
    public class ImagenConfiguracion : IEntityTypeConfiguration<Imagen>
    {
        public void Configure(EntityTypeBuilder<Imagen> builder)
        {
            builder.ToTable("Imagenes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NombreOriginal)
                .IsRequired();

            builder.Property(x => x.NombreArchivo)
                .IsRequired();

            builder.Property(x => x.RutaRelativa)
                .IsRequired();

            builder.Property(x => x.TipoContenido)
                .IsRequired();

            builder.Property(x => x.TamanoBytes)
                .IsRequired();

            builder.Property(x => x.FechaCreacion)
                .IsRequired();

            builder.HasOne(x => x.Producto)
                .WithOne(x => x.Imagen)
                .HasForeignKey<Imagen>(x => x.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ProductoId)
                .IsUnique();
        }
    }
}
