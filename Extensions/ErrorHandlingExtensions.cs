using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TablexAPI.Lib.Consts;
using TablexAPI.Models;

namespace TablexAPI.Extensions;

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder UseApiErrorHandling(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            catch (ApiException ex)
            {
                await WriteErrorAsync(context, ex.StatusCode, ex.Message);
            }
            catch (DbUpdateException ex) when (IsConstraintViolation(ex))
            {
                GetLogger(context).LogWarning(ex, "Violación de restricción en la base de datos");
                await WriteErrorAsync(context, StatusCodes.Status409Conflict, ErrorMessages.ConstraintViolation);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                GetLogger(context).LogError(ex, "Error no controlado en {Method} {Path}", context.Request.Method, context.Request.Path);
                await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, ErrorMessages.InternalServerError);
            }
        });
    }

    private static bool IsConstraintViolation(DbUpdateException ex) =>
        ex.InnerException is SqlException sql && sql.Number is 547 or 2601 or 2627;

    private static ILogger GetLogger(HttpContext context) =>
        context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("TablexAPI.Errors");

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new ApiError(statusCode, message));
    }
}
