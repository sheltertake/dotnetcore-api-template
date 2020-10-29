using FootballContacts.Contexts;
using FootballContacts.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FriendsApi.Host.Ioc
{
    public static class ServiceRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<FriendContext>();
            services.AddScoped<IFriendsRepository, FriendsRepository>();
            return services;
        }

        public static IServiceCollection AddDabaseConnection(this IServiceCollection services, string connectionString)
        {
            services.AddDbContextPool<FriendContext>(options => options.UseSqlServer(connectionString));
            return services;
        }
    }
}
