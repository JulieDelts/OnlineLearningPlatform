using System.Text.Json.Serialization;
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
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); ;
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateCourseRequest>();
        services.AddAutoMapper(typeof(APIUserMapperProfile).Assembly);
    }
}
