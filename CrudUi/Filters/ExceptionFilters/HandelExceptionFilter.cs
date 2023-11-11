using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;

namespace CrudUi.Filters.ExceptionFilters
{
    public class HandelExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HandelExceptionFilter> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public HandelExceptionFilter(ILogger<HandelExceptionFilter> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError("------Exception filter {FilterName}.{MethodName}\n{ExceptionType}\n{ExceptionMessage}", nameof(HandelExceptionFilter), nameof(OnException), context.Exception.GetType().ToString(), context.Exception.Message);

            if (_hostEnvironment.IsDevelopment())
                context.Result = new ContentResult() { Content = context.Exception.Message, StatusCode = 500 };
        }
    }
}