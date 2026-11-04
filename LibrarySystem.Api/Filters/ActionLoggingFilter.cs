using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace LibrarySystem.Api.Filters
{
    public class ActionLoggingFilter : IAsyncActionFilter
    {
        private readonly ILogger<ActionLoggingFilter> _logger;

        public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
        {
                _logger = logger;            
        }
        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var controllerName = context.RouteData.Values["controller"]?.ToString()  ?? "unknown" ;

            var actionName = context.RouteData.Values["action"]?.ToString() ?? "unknown";

            var stopwatch = Stopwatch.StartNew();


            try
            {
                var excutedContext = await next();

                if(excutedContext.Exception is not null && !excutedContext.ExceptionHandled)
                {
                    _logger.LogError(excutedContext.Exception,
                    "Starting action {Controller}.{Action}",
                    controllerName,
                    actionName);
                }
                else
                {
                    _logger.LogInformation(
                        "Action {Controller}.{Action} completed",
                        controllerName,
                        actionName);
                }
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation(
                  "Action {Controller}.{Action} took {ElapsedMilliseconds} ms",
                  controllerName,
                  actionName,
                  stopwatch.ElapsedMilliseconds);

            }
        }
    }
}
