namespace DynamicReporting.Api.Shared;

/// <summary>
/// کلاس مدیریت اکسپشن ها بصورت عمومی
/// </summary>
/// <param name="next"></param>
public sealed class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var endpoint = context.GetEndpoint();
            var actionDescriptor = endpoint?
                .Metadata
                .GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

            Log.Error(ex,
                "ExceptionType: {ExceptionType} | Path: {Path} | Controller: {Controller} | Action: {Action} | HTTPMethod: {Method} |  MethodName: {MethodName}",
                ex.GetType().FullName,
                context.Request.Path,
                actionDescriptor?.ControllerName,
                actionDescriptor?.ActionName,
                context.Request.Method,
                GetCurrentMethodName());

            var response = new
            {
#pragma warning disable IDE0037
                Message = ex.Message,
#pragma warning restore IDE0037

                ExceptionType = ex.GetType().FullName
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    /// <summary>
    /// نام متد کلاس
    /// </summary>
    /// <param name="memberName"></param>
    /// <returns></returns>
    private static string GetCurrentMethodName(
        [CallerMemberName] string memberName = "") =>
        memberName;
}