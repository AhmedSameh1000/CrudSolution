using CrudUi.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContract.DTOs;

namespace CrudUi.Filter.ResultFilters
{
    public class IndexResultFilter : IAsyncActionFilter
    {
        private readonly ILogger<IndexResultFilter> _logger;

        public IndexResultFilter(ILogger<IndexResultFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("Result Filter Before OnResultExecutionAsync");

            await next();

            //access respone body heare
            //if (context.Controller is PersonController personController)
            //{
            //    context.Result = personController.View(new List<PersonForReturnDTO>() {
            //    new PersonForReturnDTO{Age=0,Country="NoCountry",DateOfBirth=DateTime.Now,Email="Unknown.email.com",Id=Guid.NewGuid(),Name="No Name",ReceiveEmails=false,Gender="NoGender"},
            //    });
            //}
            _logger.LogInformation("Result Filter Before OnResultExecutionAsync");
        }
    }
}