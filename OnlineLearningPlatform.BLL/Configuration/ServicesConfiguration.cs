using Microsoft.Extensions.DependencyInjection;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.Mappings;

namespace OnlineLearningPlatform.BLL.Configuration;

public static class ServicesConfiguration
{
    public static void AddBLLServices(this IServiceCollection services)
    {
        services.AddScoped<ICoursesService, CoursesService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IEnrollmentsService, EnrollmentsService>();
        services.AddAutoMapper(typeof(BLLUserMapperProfile).Assembly);
    }
}
