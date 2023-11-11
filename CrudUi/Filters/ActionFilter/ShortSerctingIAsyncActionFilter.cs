using CrudUi.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContract.Interfaces;

namespace CrudUi.Filter.ActionFilter
{
    public class ShortSerctingIAsyncActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;

        public ShortSerctingIAsyncActionFilter(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonController personController)
            {
                if (!personController.ModelState.IsValid)
                {
                    /*
                    Normal way that we pass list to view and loop for and diplay it
                     var Countries = _countriesService.GetAllCoutries();
                     ViewBag.Countries = Countries;
                     */
                    var Countries = await _countriesService.GetAllCoutries();
                    personController.ViewBag.Countries = Countries.Select(c => new SelectListItem()
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                    });

                    personController.ViewBag.Errors = personController.ModelState.Values.SelectMany(c => c.Errors)
                        .Select(v => v.ErrorMessage).ToList();
                    var personModel = context.ActionArguments["personModel"];
                    //if we nedd acces argument in multyaple action they shoud be the same name like
                    //personModel in Create and Update Action methode
                    context.Result = personController.View();
                    //return personController.View();
                }
                else
                {
                    //else is already valid then go to execute action methode
                    await next();
                }
            }
        }
    }
}