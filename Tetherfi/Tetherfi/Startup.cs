using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Tetherfi.Commmon.SwaggerDocFilter;
using Tetherfi.Common.Middlewares;
using Tetherfi.Data;
using Tetherfi.Data.Models;
using Tetherfi.Helpers.JWT;
using Tetherfi.Service.Interfaces;
using Tetherfi.Services;

namespace Tetherfi;

public class Startup
{
    public IConfiguration _configuration { get; }

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // injecting DbContext to the application
        services.AddDbContext<DbContext, ApplicationDbContext>(options =>
        {
            options.UseSqlServer(_configuration.GetConnectionString("default"));
        }
        );

        // for Identity  
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // adding Authentication  
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })

        // adding Jwt Bearer  
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _configuration["JWTConfiguration:Audience"],
                ValidIssuer = _configuration["JWTConfiguration:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTConfiguration:Key"]))
            };
        });

        // injecting swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(this._configuration.GetValue<string>("Swagger:APIVersion"),
                new OpenApiInfo
                {
                    Title = this._configuration.GetValue<string>("Swagger:APITitle"),
                    Version = this._configuration.GetValue<string>("Swagger:APIVersion")
                });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter into field the word 'Bearer' following by space and JWT",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "Bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            });

            c.OperationFilter<AuthResponsesOperationFilter>();

        });

        // auto mapper config
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // injecting configurations
        services.Configure<JWTConfigurations>(_configuration.GetSection("JWTConfiguration"));

        // configure DI for application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        // enable CORS
        services.AddCors(o => o.AddPolicy("AllowSpecificOrigin", builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }));

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();


        app.UseCors("AllowSpecificOrigin");

        app.UseSwagger(c =>
        {
            c.SerializeAsV2 = true;
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(this._configuration.GetValue<string>("Swagger:SwaggerUrlRelease"),
                string.Concat(this._configuration.GetValue<string>("Swagger:APITitle"), ' ', this._configuration.GetValue<string>("Swagger:APIVersion")));
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseResponseWrapper();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", (context) => context.Response.WriteAsync("API is running..."));
            endpoints.MapControllers();
        });
    }
}
