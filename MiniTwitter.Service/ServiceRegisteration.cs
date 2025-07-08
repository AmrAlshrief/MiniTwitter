using Microsoft.Extensions.DependencyInjection;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Application.Services.Interfaces;
using MiniTwitter.Infrastructure.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Service
{
    public static class ServiceRegisteration
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<ITweetService, TweetService>();
            services.AddScoped<ITweetLikeService, TweetLikeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITimelineService, TimelineService>();
            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

            return services;
        }
    }
}
