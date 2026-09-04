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

            builder.Services.AddDbContext<Data.DataContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // De acá saqué todo esto. Me pareció interesante, así que lo puse.
            // https://codewithmukesh.com/blog/options-pattern-in-aspnet-core/
            // https://codewithmukesh.com/blog/jwt-authentication-in-aspnet-core/
            builder.Services.AddOptions<JwtSettings>()
                .BindConfiguration("JwtSettings");

            builder.Services.AddSingleton<ITokenService, TokenService>();
            builder.Services.AddScoped<IRegisterRepository, RegisterRepositoryPostgreSQL>();
            builder.Services.AddScoped<ILoginRepository, LoginRepositoryPostgreSQL>();
            builder.Services.AddScoped<ITokenService, TokenService>();



            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                
                
                app.MapOpenApi();
                app.UseSwaggerUI(opt => {
                    opt.SwaggerEndpoint("/openapi/v1.json", "API");
                });
            }

            if (app.Environment.IsDevelopment())
            {

            }

            app.UseHttpsRedirection();

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

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
