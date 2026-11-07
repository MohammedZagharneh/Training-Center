using Business.Services;


namespace APIs.ExceptionMiddleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ErrorLogService errorLogService)
        {
            try
            {
                // Continue to the next middleware/controller
                //If a downstream controller, business service, or repository throws an exception, execution jumps to the catch block.
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An unexpected error occurred."); //it shows the error in the console window

                try
                {
                    await errorLogService.LogAsync(exception, context.Request.Path, context.Request.Method);
                }
                catch (Exception loggingException)
                {
                    _logger.LogError(loggingException, "Failed to save error to the database.");
                }

                // If the response has already started, we cannot replace it.
                if (context.Response.HasStarted)
                    throw;

                context.Response.Clear();
                context.Response.StatusCode = 500;

                await context.Response.WriteAsJsonAsync(new
                {
                    Status = 500, Message = "An unexpected error occurred."
                });
            }
        }
    }
}
