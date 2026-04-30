//using System.Net;

//public class ExceptionMiddleware
//{
//    private readonly RequestDelegate _next;

//    public ExceptionMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        try
//        {
//            await _next(context);
//        }
//        catch (ArgumentException ex)
//        {
//            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

//            await context.Response.WriteAsJsonAsync(new
//            {
//                success = false,
//                message = ex.Message
//            });
//        }
//        catch (Exception)
//        {
//            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

//            await context.Response.WriteAsJsonAsync(new
//            {
//                success = false,
//                message = "Something went wrong"
//            });
//        }
//    }
//}


using System.Net;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bad request");

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = ex.Message,
                traceId = context.TraceIdentifier
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Something went wrong",
                traceId = context.TraceIdentifier
            });
        }
    }
}