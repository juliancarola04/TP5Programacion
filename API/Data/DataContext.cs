using API.Data.Configuraciones;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoriaConfiguracion());
            modelBuilder.ApplyConfiguration(new ClienteConfiguracion());
            modelBuilder.ApplyConfiguration(new DetalleIngresoConfiguracion());
            modelBuilder.ApplyConfiguration(new DetalleVentaConfiguracion());
            modelBuilder.ApplyConfiguration(new ImagenConfiguracion());
            modelBuilder.ApplyConfiguration(new IngresoConfiguracion());
            modelBuilder.ApplyConfiguration(new ProductoConfiguracion());
            modelBuilder.ApplyConfiguration(new ProveedorConfiguracion());
            modelBuilder.ApplyConfiguration(new UsuarioConfiguracion());
            modelBuilder.ApplyConfiguration(new VentaConfiguracion());
            modelBuilder.ApplyConfiguration(new UsuarioConfiguracion());

        }

        public DbSet<Categoria> Categorias{ get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<DetalleIngreso> DetallesIngresos { get; set; }
        public DbSet<DetalleVenta> DetallesVentas { get; set; }
        public DbSet<Imagen> Imagenes { get; set; }
        public DbSet<Ingreso> Ingresos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
    }
}
