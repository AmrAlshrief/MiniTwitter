
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Infrastructure;
using MiniTwitter.Infrastructure.Middleware;
using MiniTwitter.Data;
using MiniTwitter.Service;
using MiniTwitter.Core.Persistence.Interfaces;
using MiniTwitter.Data.Repositories;

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
             ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddDataServices();
            builder.Services.AddServiceLayer();
            builder.Services.AddInfrastructure();


            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            //app.UseExceptionHandling();

            app.UseAuthorization();
            
            app.MapControllers();

            app.MapGet("/test-db", async (ApplicationDbContext db) =>
            {
                var canConnect = await db.Database.CanConnectAsync();
                return canConnect ? Results.Ok("DB is reachable") : Results.Problem("DB not reachable");
            });

            var sp = app.Services.CreateScope().ServiceProvider;
            var dbContext = sp.GetService<ApplicationDbContext>();
            Console.WriteLine(dbContext != null ? "DbContext is registered." : "DbContext is MISSING!");



            app.Run();
        }
    }
}
