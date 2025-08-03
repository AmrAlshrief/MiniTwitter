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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Distributed;
using MiniTwitter.Core.Application.Services.interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;


namespace MiniTwitter.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            // Redis setup
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
            { 
                var configuration = builder.Configuration.GetConnectionString("Redis"); 
                var options = ConfigurationOptions.Parse(configuration);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options); 
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("JwtSettings"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/signin-google"; // default
            })
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

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("OnTokenValidated called");
                    
                    // Extract JTI from claims instead of SecurityToken
                    var jtiClaim = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti);
                    if (jtiClaim == null)
                    {
                        logger.LogWarning("JTI claim not found in token");
                        return;
                    }

                    var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
                    var jti = jtiClaim.Value;
                    logger.LogInformation($"Checking blacklist for JTI: {jti}");

                    if (string.IsNullOrEmpty(jti))
                    {
                        logger.LogWarning("JTI is null or empty");
                        return;
                    }

                    var blacklistKey = $"blacklist:{jti}";
                    var isBlacklisted = await cacheService.ExistsAsync(blacklistKey);
                    logger.LogInformation($"Blacklist check for key '{blacklistKey}': {isBlacklisted}");
                    
                    if (isBlacklisted)
                    {
                        logger.LogWarning($"Token with JTI {jti} is blacklisted");
                        context.Fail("Token is blacklisted.");
                    }
                    else
                    {
                        logger.LogInformation($"Token with JTI {jti} is valid (not blacklisted)");
                    }
                }
                };
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
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
            app.UseRouting();
            // app.UseHttpsRedirection();

            app.UseCors("AllowFrontend"); // Enable CORS for all endpoints

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
