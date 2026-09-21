using API.Implementacion;
using API.Options;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 1. Bootstrap Logger: captura errores incluso antes de que la config termine de cargar.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Iniciando aplicación API...");

                var builder = WebApplication.CreateBuilder(args);

                // 2. Conectar Serilog al Host, leyendo la config de appsettings.json
                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext());

                builder.Services.AddDbContext<Data.DataContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
                builder.Services.AddScoped<VentaService>();
                builder.Services.AddScoped<IngresoService>();

                builder.Services.AddSingleton<ITokenService, TokenService>();
                builder.Services.AddScoped<IRegisterRepository, RegisterRepositoryPsqlEF>();
                builder.Services.AddScoped<IProductoRepository, ProductoRepositoryPsqlEF>();
                builder.Services.AddScoped<IImagenRepository, ImagenRepositoryPsqlEF>();
                builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryPsqlEF>();
                builder.Services.AddScoped<ICategoriaRepository, CategoriaRepositoryPsqlEF>();
                builder.Services.AddScoped<IClienteRepository, ClienteRepositoryPsqlEF>();
                builder.Services.AddScoped<IProveedorRepository, ProveedorRepositoryPsqlEF>();
                builder.Services.AddScoped<IVentaRepository, VentaRepositoryPsqlEF>();
                builder.Services.AddScoped<IIngresoRepository, IngresoRepositoryPsqlEF>();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("PermitirTodo", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

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
                builder.Services.AddOpenApi();

                var app = builder.Build();
                var uploadsPath = Path.Combine(
                    app.Environment.WebRootPath
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");

                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                app.UseExceptionHandler(exceptionHandlerApp =>
                {
                    exceptionHandlerApp.Run(async context =>
                    {
                        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                        Exception? exception = exceptionFeature?.Error;

                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        if (exception is System.Data.Common.DbException dbException)
                        {
                            logger.LogError(dbException,
                                "Error de base de datos al procesar {Method} {Path}",
                                context.Request.Method, context.Request.Path);

                            await context.Response.WriteAsync("\"Ocurrió un error interno en el servidor. Por favor, intentá nuevamente más tarde.\"");
                        }
                        else
                        {
                            // Cualquier otra excepción no prevista (un bug real) también queda registrada.
                            logger.LogError(exception,
                                "Excepción no controlada al procesar {Method} {Path}",
                                context.Request.Method, context.Request.Path);

                            await context.Response.WriteAsync("\"Ocurrió un error interno en el servidor.\"");
                        }
                    });
                });

                app.UseSerilogRequestLogging(options =>
                {
                    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondió {StatusCode} en {Elapsed:0.0000} ms";
                });

                // 3. Auditoría HTTP automática de Serilog
                app.UseSerilogRequestLogging(options =>
                {
                    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondió {StatusCode} en {Elapsed:0.0000} ms";
                });

                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseCors("PermitirTodo");
                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fallo crítico no controlado durante el inicio de la aplicación.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}