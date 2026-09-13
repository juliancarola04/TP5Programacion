using API.Implementacion;
using API.Options;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.

            builder.Services.AddDbContext<Data.DataContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // De acá saqué todo esto. Me pareció interesante, así que lo puse.
            // https://codewithmukesh.com/blog/options-pattern-in-aspnet-core/
            // https://codewithmukesh.com/blog/jwt-authentication-in-aspnet-core/
            builder.Services.AddOptions<JwtSettings>()
                .BindConfiguration("JwtSettings");

            builder.Services.AddScoped<LoginService>();
            builder.Services.AddScoped<RegisterService>();
            builder.Services.AddScoped<ProductoService>();
            builder.Services.AddScoped<ImagenService>();

            builder.Services.AddScoped<UsuarioService>();
            builder.Services.AddScoped<CategoriaService>();
            builder.Services.AddScoped<ClienteService>();
            builder.Services.AddScoped<ProveedorService>();


            builder.Services.AddSingleton<ITokenService, TokenService>();
            builder.Services.AddScoped<IRegisterRepository, RegisterRepositoryPostgreSQL>();
            builder.Services.AddScoped<ILoginRepository, LoginRepositoryPostgreSQL>();
            builder.Services.AddScoped<IProductoRepository, ProductoRepositoryPostgreSQL>();
            builder.Services.AddScoped<IImagenRepository, ImagenRepositoryPostgreSQL>();

            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryPostgreSQL>();
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepositoryPostgreSQL>();
            builder.Services.AddScoped<IClienteRepository, ClienteRepositoryPostgreSQL>();
            builder.Services.AddScoped<IProveedorRepository, ProveedorRepositoryPostgreSQL>();


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<JwtSettings>>((options, jwtSettingsOptions) =>
                {
                    var jwtSettings = jwtSettingsOptions.Value;

                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();
            var uploadsPath = Path.Combine(
            app.Environment.WebRootPath
            ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllers();

            app.Run();

        }
    }
}
