using Microsoft.EntityFrameworkCore;
using MiniTwitter.Infrastructure;
using MiniTwitter.Infrastructure.Middleware;
using MiniTwitter.Data;
using MiniTwitter.Service;
using MiniTwitter.Core;
using MiniTwitter.Core.Persistence.Interfaces;
using MiniTwitter.Data.Repositories;
using MiniTwitter.Core.Configurations;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MiniTwitter.Infrastructure.ExternalServices.CloudinaryService;

namespace MiniTwitter.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
            //    {
            //        // Inside Docker - only listen on HTTP
            //        options.ListenAnyIP(8080); // HTTP only
            //    }
            //    else
            //    {
            //        // Outside Docker - use HTTPS if desired
            //        options.ListenAnyIP(5000); // HTTP
            //        options.ListenAnyIP(5001, listenOptions =>
            //        {
            //            listenOptions.UseHttps();
            //        });
            //    }
            //});

            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    options.ListenAnyIP(8080); // match EXPOSE 8080
            //});



            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
             ?? Environment.GetEnvironmentVariable("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddDataServices();
            builder.Services.AddServiceLayer();
            builder.Services.AddInfrastructure();

            // Cloudinary setup
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();


            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("JwtSettings"));

            builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                };
            });


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });



            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors(); // Enable CORS for all endpoints

            //app.UseExceptionHandling();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllers();

            app.MapGet("/test-db", async (ApplicationDbContext db) =>
            {
                Console.WriteLine($"Current environment: {app.Environment.EnvironmentName}");
                var canConnect = await db.Database.CanConnectAsync();
                return canConnect ? Results.Ok($"DB is reachable: {app.Environment.EnvironmentName}") : Results.Problem("DB not reachable");
            });

            var sp = app.Services.CreateScope().ServiceProvider;
            var dbContext = sp.GetService<ApplicationDbContext>();
            Console.WriteLine(dbContext != null ? "DbContext is registered." : "DbContext is MISSING!");

            app.MapGet("/jwt-debug", (IOptions<JwtSettings> jwt) =>
            {
                Console.WriteLine($"Current environment: {app.Environment.EnvironmentName}");
                return Results.Ok(jwt.Value);
            });

            


            app.Run();
        }
    }
}
