using CRUD_Example.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD_Example.Controllers
{
    [Route("Persons")]
    [TypeFilter(typeof(ResponseHeaderFilter), Arguments = new object[] { "X_Controller_key", "X_Controller6_value",3 })]
    public class PersonsController : Controller
    {

        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logging;


        public PersonsController(IPersonService personService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personService = personService;
            _countriesService = countriesService;
            _logging = logger;
        }
        [Route("/")]
        [Route("[action]")]
        [TypeFilter(typeof(PersonsListActionFilter),Order =1)]
        [TypeFilter(typeof(ResponseHeaderFilter),Arguments =new object[] {"X_Action_key","X_Action_value",1})]  
        public async Task<IActionResult> Index(string SearchBy, string? SearchString, string SortBy = nameof(PersonResponse.PersonName), SortOrderOptions SortOrder = SortOrderOptions.ASD)
        {

            _logging.LogDebug("Index method");
            

            //searching

            List<PersonResponse> PersonList = await _personService.GetFilteredPersons(SearchBy, SearchString);

            //ViewBag.CurrentSearchBy = SearchBy;
            //ViewBag.CurrentSearchString = SearchString;



            //sorting
            List<PersonResponse> sortedList = await _personService.GetSortedPersons(PersonList, SortBy, SortOrder);
            //ViewBag.CurrentSortBy = SortBy;
            //ViewBag.CurrentSortOrder = SortOrder.ToString();


            return View(sortedList);
        }

        [Route("[action]")]
        [HttpGet]
        [TypeFilter(typeof(ResponseHeaderFilter), Arguments = new object[] { "X_Action_key", "X_Action_value" ,1})]
        public async Task<IActionResult> Create()
        {
           

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
              new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() }
            );
            /*
            new selectListItems(){
            Text="Gokul" ,value="1"}

            <option value="1">Gokul</option>
             * */

            return View();
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.CountriesList = await _countriesService.GetAllCountries();
                ViewBag.Errors=ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();

                return View();
            }

           await _personService.AddPerson(personAddRequest);

            
            return RedirectToAction("Index","Persons");
        }


        [Route("[action]/{PersonID}")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? PersonID)
        {
           PersonResponse personResponse= await _personService.GetPersonByPersonID(PersonID);

            
          if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            List<CountryResponse> countries = await  _countriesService.GetAllCountries();
            ViewBag.countries = countries.Select(temp => new SelectListItem()
            {
              Text = temp.CountryName,Value = temp.CountryID.ToString()
            });

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            return View(personUpdateRequest);
        }

        [Route("[action]/{personId}")]
        [HttpPost]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {

            PersonResponse personResponse=await _personService.GetPersonByPersonID(personUpdateRequest.PersonID);

           if(personResponse == null) { 
                    
              return RedirectToAction("index"); ; 
            }

           if(ModelState.IsValid)   
            {
                await _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("index");
            }
            else
            {
                ViewBag.Errors = ModelState.Values.SelectMany(e=>e.Errors).Select(e=>e.ErrorMessage).ToList();
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.countries = countries.Select(temp => new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryID.ToString()
                });


                return View(personResponse.ToPersonUpdateRequest());

            }
        }


        [Route("[action]/{PersonID}")]
        [HttpGet] 
        public async Task<IActionResult> Delete(Guid PersonID) {

           PersonResponse personResponse= await _personService.GetPersonByPersonID(PersonID);

            if(personResponse == null)
            {
                return RedirectToAction("index");   
            }
                
           return  View(personResponse);    
                
        }
        [Route("[action]/{PersonID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(PersonResponse personResponse)
        {

            PersonResponse person = await _personService.GetPersonByPersonID(personResponse.PersonID);

            if (person == null)
            {
                return RedirectToAction("index");
            }

            await _personService.DeletePerson(personResponse.PersonID);
            return RedirectToAction("index");

        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
            var model = await _personService.GetAllPersons();

            return new ViewAsPdf("PersonsPDF", model, ViewData)
            {
                PageMargins=new Margins()
                {
                    Top=20,Bottom=20,Right=20,Left=20
                },
                PageOrientation=Orientation.Landscape

            };
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream=await _personService.GetPersonsCSV();

            return File(memoryStream, "applcation/octet-stream", "Persons.CSV");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream=await _personService.GetPersonsExcel();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Persons.xlsx");
        }
        
    }
}
