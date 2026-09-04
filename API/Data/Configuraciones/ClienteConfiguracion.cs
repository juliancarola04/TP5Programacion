using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Data.Configuraciones
{
    public class ClienteConfiguracion : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Dni)
                .IsUnique();
            
            builder.HasIndex(x => x.Email)
                .IsUnique();
            
            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(30);
            
            builder.Property(x => x.Dni)
                .IsRequired()
                .HasMaxLength(8);
            
            builder.Property(x => x.Telefono)
                .IsRequired()
                .HasMaxLength(13);
            
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(320);
            
            builder.Property(x => x.Direccion)
                .IsRequired()
                .HasMaxLength(40);

        }
    }
}
