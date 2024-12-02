using FluentValidation;
using FluentValidation.AspNetCore;
using OnlineLearningPlatform.Mappings;
using OnlineLearningPlatform.Models.Requests;

namespace OnlineLearningPlatform.Configuration;

public static class ServicesConfiguration
{
    public static void AddAPIServices(this IServiceCollection services)
    {
        services.AddAuth();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateCourseRequest>();
        services.AddAutoMapper(typeof(APIUserMapperProfile).Assembly);
    }
}
