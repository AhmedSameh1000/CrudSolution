using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudUi.Filter.ResourceFilters
{
    public class DisabledResourseFilter : IAsyncResourceFilter
    {
        private readonly ILogger<DisabledResourseFilter> _logger;
        private readonly bool _isDisabled;

        public DisabledResourseFilter(ILogger<DisabledResourseFilter> logger,
            bool IsDisabled = true)
        {
            _logger = logger;
            _isDisabled = IsDisabled;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (_isDisabled)
            {
                _logger.LogInformation(" DisabledResourseFilter before OnResourceExecutionAsync");
                //context.Result = new StatusCodeResult(501);
                //context.Result = new RedirectResult("NotFound");
                //do any thing like for example guard in angular
            }
            else
            {
                await next();
            }
            _logger.LogInformation(" DisabledResourseFilter after OnResourceExecutionAsync");
        }
    }
}