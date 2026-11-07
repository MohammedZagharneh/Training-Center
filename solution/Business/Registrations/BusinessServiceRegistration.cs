using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.Connections;
using Business.Services;
using DataAccess.Repositories;


namespace Business.Registrations
{
    public static class BusinessServiceRegistration
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            //ef and ado
            services.AddDbContext<TrainingCenterContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<SqlConnectionFactory>();

            //error Log
            services.AddScoped<ErrorLogRepository>();
            services.AddScoped<ErrorLogService>();


            return services;
        }
    }
}
