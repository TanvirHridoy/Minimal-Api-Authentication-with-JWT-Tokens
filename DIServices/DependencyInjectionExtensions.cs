using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.DTO;
using MinimalApi.Repository;
using MinimalApi.Services;
using System.Text;

namespace MinimalApi.DIServices;

public static class DependencyInjectionExtensions
{

    public static IServiceCollection AppApplicationServices(this IServiceCollection services,IConfiguration config)
    {
        services.AddDbContext<EmployeeDbContext>(o => o.UseSqlServer(config.GetConnectionString("EmployeeDb")));
        services.AddIdentity<ApplicationUser, ApplicationUserRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
    .AddEntityFrameworkStores<EmployeeDbContext>()
    .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "JwtBearer";
            options.DefaultChallengeScheme = "JwtBearer";
        }).AddJwtBearer("JwtBearer", jwtoptions =>
        {
            jwtoptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:EncryptionKey"]))
            };
        });
        services.AddAuthorization();

        services.AddScoped<JwtTokenService>();

        services.AddScoped<EmployeeRepository>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}
