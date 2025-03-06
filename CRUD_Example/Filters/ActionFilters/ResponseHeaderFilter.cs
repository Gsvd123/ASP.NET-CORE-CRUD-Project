using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_Example.Filters.ActionFilters
{
    public class ResponseHeaderFilter : IAsyncActionFilter,IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderFilter> _logger;
        private readonly string key;
        private readonly string value;

        public int Order;

        public ResponseHeaderFilter(ILogger<ResponseHeaderFilter> logger, string key, string value, int order)
        {
            _logger = logger;
            this.key = key;
            this.value = value;
            Order = order;
        }

        int IOrderedFilter.Order { get; }

        
         async Task  IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{filterName}.{MethodName} method ^^^^^^^^", nameof(ResponseHeaderFilter), nameof(IActionFilter.OnActionExecuted));
            await next();
            _logger.LogInformation("{filterName}.{MethodName} method ^^^^^^^^", nameof(ResponseHeaderFilter), nameof(IActionFilter.OnActionExecuting));
            context.HttpContext.Response.Headers[key] = value;
        }
    }
}
