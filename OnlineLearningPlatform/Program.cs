using FluentValidation;
using FluentValidation.AspNetCore;
using OnlineLearningPlatform.BLL;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.Configuration;
using OnlineLearningPlatform.DAL;
using OnlineLearningPlatform.DAL.Interfaces;
using OnlineLearningPlatform.Models.Requests;

namespace OnlineLearningPlatform;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuth();

        builder.Services.AddScoped<ICoursesService, CoursesService>();
        builder.Services.AddScoped<IUsersService, UsersService>();

        builder.Services.AddScoped<ICoursesRepository, CoursesRepository>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IEnrollmentsRepository, EnrollmentsRepository>();
        builder.Services.AddScoped<OnlineLearningPlatformContext>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateCourseRequest>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ExceptionMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
