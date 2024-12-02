using OnlineLearningPlatform.BLL.Configuration;
using OnlineLearningPlatform.Configuration;
using OnlineLearningPlatform.DAL.Configuration;

namespace OnlineLearningPlatform;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args)
            .AddEnvironmentVariables()
            .Build();

        var configuration = builder.Configuration;

        builder.Services.AddDALServices(configuration);
        builder.Services.AddBLLServices();
        builder.Services.AddAPIServices();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ExceptionMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
