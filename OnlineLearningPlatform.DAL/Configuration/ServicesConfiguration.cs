using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.DAL.Configuration;

public static class ServicesConfiguration
{
    public static void AddDALServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICoursesRepository, CoursesRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IEnrollmentsRepository, EnrollmentsRepository>();
        services.AddDbContext<OnlineLearningPlatformContext>(options => options.UseNpgsql(configuration.GetConnectionString("OnlineLearningPlatformConnection")));
    }
}
