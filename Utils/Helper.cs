using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Utils;

public class Helper(IConfiguration configuration, IServiceCollection services)
{
    private readonly IConfiguration _config = configuration;
    private readonly IServiceCollection _service = services;

    public void SwaggerConfig()
    {
        _service.AddSwaggerGen((options) =>
        {
            options.SwaggerDoc("V1", new OpenApiInfo { Title = "Task API", Version = "V1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter bearer token {token}",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme{
                        Reference= new OpenApiReference{
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public void DbConnectionCtx()
    {
        var connection_db = _config.GetConnectionString("DefaultConnection");
        _service.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection_db));
    }

    public void IdentityAdding()
    {
        _service.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
    }

    public void AddAuthentication()
    {
        var jwt = _config.GetSection("Jwt");
        _service.AddAuthentication(op =>
        {
            op.DefaultChallengeScheme = "Bearer";
            op.DefaultAuthenticateScheme = "Bearer";
        }).AddJwtBearer(bearer =>
        {
            bearer.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["JwtSecret"]!))
            };
        });
    }
}
