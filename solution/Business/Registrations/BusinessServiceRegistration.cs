using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.Connections;
using Business.Services;
using DataAccess.Repositories;
using DataAccess.TransactionManager;


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

            //Person
            services.AddScoped<PersonRepository>();
            services.AddScoped<PersonService>();

            //Student
            services.AddScoped<StudentRepository>();
            services.AddScoped<StudentService>();

            //Instructor
            services.AddScoped<InstructorRepository>();
            services.AddScoped<InstructorService>();

            //Course
            services.AddScoped<CourseRepository>();
            services.AddScoped<CourseService>();

            //Enrollment
            services.AddScoped<EnrollmentRepository>();
            services.AddScoped<EnrollmentService>();

            //User
            services.AddScoped<AuthRepository>();
            services.AddScoped<AuthService>();

            //Transaction Manager
            services.AddScoped<EfTransactionManager>();

            return services;
        }
    }
}
