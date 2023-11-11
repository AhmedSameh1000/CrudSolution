using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudUi.Filter.ActionFilter
{
    public class PersonListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonListActionFilter> _logger;

        public PersonListActionFilter(ILogger<PersonListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var data = context.HttpContext.Request.Query["searchBy"];
            var data2 = context.HttpContext.Request.Query["searchString"];
            data = "ahmed";
            data2 = "ali";
            context.HttpContext.Response.Headers.Add("Name", "Ahmed");
            _logger.LogInformation("PersonListActionFilter.OnActionExecuted");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var data = context.HttpContext.Request.Query["searchBy"];
            var data2 = context.HttpContext.Request.Query["searchString"];
            data = "ahmed";
            data2 = "ali";

            _logger.LogInformation("PersonListActionFilter.OnActionExecuting");
        }
    }
}