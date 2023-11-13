using CrudUi.Filter.ActionFilter;
using CrudUi.Filter.ResourceFilters;
using CrudUi.Filter.ResultFilters;
using CrudUi.Filters.AutherizationFilters;
using CrudUi.Filters.ResultFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContract.DTOs;
using ServiceContract.Enums;
using ServiceContract.Interfaces;

namespace CrudUi.Controllers
{
    [Authorize]
    //Glopal Filter for all methode
    //[TypeFilter(typeof(HandelExceptionFilter))]
    [TypeFilter(typeof(ResponseHeaderActionFilter),
           Arguments = new object[] { "Controler-key", "Controler-value" })]
    public class PersonController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonService _personService;
        private readonly ILogger<PersonController> _logger;

        public PersonController(
            ICountriesService countriesService,
            IPersonService personService,
            ILogger<PersonController> logger)
        {
            _countriesService = countriesService;
            _personService = personService;
            _logger = logger;
        }

        [AllowAnonymous]
        [Route("")]
        [ServiceFilter(typeof(PersonListActionFilter))]//Service Filter is aservice we should log it in services and its lifetime
        [TypeFilter(typeof(ResponseHeaderActionFilter),
            Arguments = new object[] { "Index-key", "Index-value" })]//type filter by default is transient service
        [TypeFilter(typeof(IndexResultFilter))]
        public async Task<IActionResult> Index(string? searchBy, string? searchString,
            string sortBy = (nameof(PersonForReturnDTO.Name)), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index Action Methode in Person Controller");
            _logger.LogDebug($"Sort By{sortBy} || Sort Order {sortOrder}");
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                {nameof(PersonForReturnDTO.Name) ,"Person Name" },
                {nameof(PersonForReturnDTO.Email) ,"Email" },
                {nameof(PersonForReturnDTO.DateOfBirth) ,"Date Of Birth" },
                {nameof(PersonForReturnDTO.Gender) ,"Gender" },
                {nameof(PersonForReturnDTO.Country) ,"Country" },
            };
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            //Search
            var Persons = await _personService.GetFilteredPersons(searchBy, searchString);
            //Sort
            var SortedPersons = _personService.GetSortedPersons(Persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(SortedPersons);
        }

        [Route("Person/Create")]
        [HttpGet]
        [TypeFilter(typeof(ResponseHeaderActionFilter),
            Arguments = new object[] { "create-key", "create-value" })]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create Action Methode in Person Controller");
            /*
             Normal way that we pass list to view and loop for and diplay it
              var Countries = _countriesService.GetAllCoutries();
              ViewBag.Countries = Countries;
             */
            var Countries = await _countriesService.GetAllCoutries();
            ViewBag.Countries = Countries.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
            return View();
        }

        [Route("Person/Create")]
        [HttpPost]
        [TypeFilter(typeof(ShortSerctingIAsyncActionFilter))]
        public async Task<IActionResult> Create(PersonForCreateDTO personModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    /*
            //    Normal way that we pass list to view and loop for and diplay it
            //     var Countries = _countriesService.GetAllCoutries();
            //     ViewBag.Countries = Countries;
            //     */
            //    var Countries = await _countriesService.GetAllCoutries();
            //    ViewBag.Countries = Countries.Select(c => new SelectListItem()
            //    {
            //        Text = c.Name,
            //        Value = c.Id.ToString(),
            //    });

            //    ViewBag.Errors = ModelState.Values.SelectMany(c => c.Errors)
            //        .Select(v => v.ErrorMessage).ToList();
            //    return View();
            //}

            await _personService.AddPerson(personModel);

            return RedirectToAction("Index", "Person");
        }

        [Route("Person/Update/{id}")]
        [HttpGet]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Update(Guid id)
        {
            _logger.LogInformation("Update Action Methode in Person Controller");

            var PersonForReturn = await _personService.GetPersonById(id);
            if (PersonForReturn is null)
            {
                return RedirectToAction("Index");
            }
            var PersonForUpdate = PersonForReturn.ToPersonForUpdateDTO();

            /*
             Normal way that we pass list to view and loop for and diplay it
              var Countries = _countriesService.GetAllCoutries();
              ViewBag.Countries = Countries;
             */
            var Countries = await _countriesService.GetAllCoutries();
            ViewBag.Countries = Countries.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
            return View(PersonForUpdate);
        }

        [Route("Person/Update/{id}")]
        [HttpPost]
        [TypeFilter(typeof(ShortSerctingIAsyncActionFilter))]
        [TypeFilter(typeof(TokenAutherizationFilter))]
        public async Task<IActionResult> Update(PersonForUpdateDTO personModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    /*
            //    Normal way that we pass list to view and loop for and diplay it
            //     var Countries = _countriesService.GetAllCoutries();
            //     ViewBag.Countries = Countries;
            //     */
            //    var Countries = await _countriesService.GetAllCoutries();
            //    ViewBag.Countries = Countries.Select(c => new SelectListItem()
            //    {
            //        Text = c.Name,
            //        Value = c.Id.ToString(),
            //    });

            //    ViewBag.Errors = ModelState.Values.SelectMany(c => c.Errors)
            //        .Select(v => v.ErrorMessage).ToList();
            //    return View();
            //}

            await _personService.UpdatePerson(personModel);

            return RedirectToAction("Index", "Person");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Person/Delete/{id}")]
        [TypeFilter(typeof(ResponseHeaderAsyncActionFilter),
            Arguments = new object[] { "Delete-key", "Delete-value" })]
        [TypeFilter(typeof(DisabledResourseFilter), Arguments =
            new object[] { false })]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Delete Action Methode in Person Controller");

            var PersonToDelete = await _personService.GetPersonById(id);

            if (PersonToDelete is null)
            {
                return RedirectToAction("Index");
            }

            return View(PersonToDelete);
        }

        [HttpPost]
        [Route("Person/Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, PersonForReturnDTO personForReturn)
        {
            var PersonToDelete = await _personService.GetPersonById(id);

            if (PersonToDelete is null)
                return RedirectToAction("Index");

            await _personService.RemovePerson(id);
            return RedirectToAction("Index");
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }
    }
}