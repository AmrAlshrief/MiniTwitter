using Microsoft.Extensions.DependencyInjection;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Persistence.Interfaces;
using MiniTwitter.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Data
{
    public static class DataServiceRegisteration
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            /*services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));*/
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ITweetLikeRepository, TweetLikeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITweetRepository, TweetRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IFollowRepository, FollowRepository>();
            return services;
        }
    }
}
