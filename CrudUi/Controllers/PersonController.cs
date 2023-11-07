using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContract.DTOs;
using ServiceContract.Enums;
using ServiceContract.Interfaces;
using Services;

namespace CrudUi.Controllers
{
    public class PersonController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonService _personService;

        public PersonController(
            ICountriesService countriesService,
            IPersonService personService)
        {
            _countriesService = countriesService;
            _personService = personService;
        }

        [Route("")]
        public async Task<IActionResult> Index(string? searchBy, string? searchString,
            string sortBy = (nameof(PersonForReturnDTO.Name)), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
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
        public async Task<IActionResult> Create()
        {
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
        public async Task<IActionResult> Create(PersonForCreateDTO personForCreate)
        {
            if (!ModelState.IsValid)
            {
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

                ViewBag.Errors = ModelState.Values.SelectMany(c => c.Errors)
                    .Select(v => v.ErrorMessage).ToList();
                return View();
            }

            await _personService.AddPerson(personForCreate);

            return RedirectToAction("Index", "Person");
        }

        [Route("Person/Update/{id}")]
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
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
        public async Task<IActionResult> Update(PersonForUpdateDTO personForCreate)
        {
            if (!ModelState.IsValid)
            {
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

                ViewBag.Errors = ModelState.Values.SelectMany(c => c.Errors)
                    .Select(v => v.ErrorMessage).ToList();
                return View();
            }

            await _personService.UpdatePerson(personForCreate);

            return RedirectToAction("Index", "Person");
        }

        [HttpGet]
        [Route("Person/Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var PersonToDelete = await _personService.GetPersonById(id);

            if (PersonToDelete is null)
                return RedirectToAction("Index");

            return View(PersonToDelete);
        }

        [HttpPost]
        [Route("Person/Delete/{id}")]
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