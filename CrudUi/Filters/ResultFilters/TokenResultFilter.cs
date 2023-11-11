using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudUi.Filters.ResultFilters
{
    public class TokenResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            context.HttpContext.Response.Cookies.Append("Token-Key", "Ahmed");
           //the same code Cookies will be readOnly 
            
            await next();
           // context.HttpContext.Response.Cookies.Append("Token-Key", "Ahmed"); Cookies and Header readOnly
           
        }
    }
}