using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudUi.Filter.ActionFilter
{
    public class ResponseHeaderActionFilter : IActionFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string _key;
        private readonly string _value;

        public ResponseHeaderActionFilter(
            ILogger<ResponseHeaderActionFilter> logger
            , string key, string value)
        {
            _logger = logger;
            _key = key;
            _value = value;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Headers[_key] = _value;
            _logger.LogInformation("ResponseHeaderActionFilter.OnActionExecuted");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("PersonListActionFilter.OnActionExecuting");
        }
    }
}

/*
 3 levels
Action Level
Class Level
ProgramLevel Or Glopal Level
 */