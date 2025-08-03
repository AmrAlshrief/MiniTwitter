using Microsoft.Extensions.DependencyInjection;
using MiniTwitter.Core.Application.Events;
using MiniTwitter.Infrastructure.Events.Handlers;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Domain.Events;
using MiniTwitter.Infrastructure.Authentication;
using MiniTwitter.Infrastructure.Email;
using MiniTwitter.Infrastructure.Events;
using MiniTwitter.Infrastructure.Logging;
using MiniTwitter.Infrastructure.Middleware;
using MiniTwitter.Infrastructure.Caching;
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
            //services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IEventDispatcher, InMemoryEventDispatcher>();
            services.AddScoped<IEventHandler<TweetLikedEvent>, SendLikeNotificationHandler>();
            services.AddScoped<IEventHandler<TweetRetweetedEvent>, SendRetweetNotificationHandler>();
            services.AddScoped<IEventHandler<UserFollowedEvent>, SendFollowNotificationHandler>();
            services.AddScoped<ICacheService, RedisCacheService>();
            return services;
        }
    }
}
