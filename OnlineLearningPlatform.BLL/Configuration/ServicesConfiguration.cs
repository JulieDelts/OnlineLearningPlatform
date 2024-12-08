using Microsoft.Extensions.DependencyInjection;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.BLL.ServicesUtils;

namespace OnlineLearningPlatform.BLL.Configuration;

public static class ServicesConfiguration
{
    public static void AddBLLServices(this IServiceCollection services)
    {
        services.AddScoped<ICoursesService, CoursesService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IEnrollmentsService, EnrollmentsService>();
        services.AddScoped<CoursesUtils>();
        services.AddScoped<UsersUtils>();
        services.AddScoped<EnrollmentsUtils>();
        services.AddAutoMapper(typeof(BLLUserMapperProfile).Assembly);
    }
}
