using Microsoft.Extensions.DependencyInjection;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Infrastructure.Authentication;
using MiniTwitter.Infrastructure.Email;
using MiniTwitter.Infrastructure.Logging;
using MiniTwitter.Infrastructure.Middleware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Infrastructure
{
    public static class InfrastructureServiceRegisteration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            //services.AddScoped<ExceptionHandlingMiddleware>();
            return services;
        }
    }
}
