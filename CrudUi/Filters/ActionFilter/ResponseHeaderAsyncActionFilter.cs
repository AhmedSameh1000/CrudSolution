using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudUi.Filter.ActionFilter
{
    public class ResponseHeaderAsyncActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<ResponseHeaderAsyncActionFilter> _logger;
        private readonly string _key;
        private readonly string _value;

        public ResponseHeaderAsyncActionFilter(ILogger<ResponseHeaderAsyncActionFilter> logger, string key, string value)
        {
            _logger = logger;
            _key = key;
            _value = value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("ResponseHeaderAsyncActionFilter Befor");
            await next();
            _logger.LogInformation("ResponseHeaderAsyncActionFilter After");

            context.HttpContext.Response.Headers[_key] = _value;
        }
    }
}