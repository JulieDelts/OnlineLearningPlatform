using OnlineLearningPlatform.BLL.Exceptions;
using System.Net;

namespace OnlineLearningPlatform.Configuration;

internal class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            httpContext.Response.ContentType = "application/json";
            await _next(httpContext);
        }
        catch (EntityNotFoundException ex)
        {
            await HandleEntityNotFoundExceptionAsync(httpContext, ex);
        }
        catch (EntityConflictException ex)
        {
            await HandleEntityConflictExceptionAsync(httpContext, ex);
        }
        catch (WrongCredentialsException ex)
        {
            await HandleWrongCredentialsExceptionAsync(httpContext, ex);
        }
        catch (AuthorizationFailedException ex)
        {
            await HandleAuthorizationFailedExceptionAsync(httpContext, ex);
        }
        catch (ArgumentException ex)
        {
            await HandleArgumentExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleEntityNotFoundExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        await WriteErrorDetailsAsync(context, exception.Message);
    }

    private async Task HandleEntityConflictExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Conflict;
        await WriteErrorDetailsAsync(context, exception.Message);
    }

    private async Task HandleArgumentExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await WriteErrorDetailsAsync(context, exception.Message);
    }

    private async Task HandleWrongCredentialsExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        await WriteErrorDetailsAsync(context, exception.Message);
    }

    private async Task HandleAuthorizationFailedExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        await WriteErrorDetailsAsync(context, exception.Message);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await WriteErrorDetailsAsync(context, "Бэкендер обосрался!");
    }

    private async Task WriteErrorDetailsAsync(HttpContext context, string message)
    {
        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = message
        }.ToString());
    }
}
