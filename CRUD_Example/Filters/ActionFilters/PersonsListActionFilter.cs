using CRUD_Example.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUD_Example.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
    

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
             
        }
        
        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {

            //_logger.LogInformation("PersonsListActionFilter.OnActionExecuted methods ************** ");
            _logger.LogInformation("{FilterName}.{ActionMethod} methods ******************",nameof(PersonsListActionFilter),nameof(IActionFilter.OnActionExecuted));

            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                {nameof(PersonResponse.PersonName),"Person Name" },
                 {nameof(PersonResponse.Email),"Email Address" },
                  {nameof(PersonResponse.DateOfBirth),"Date of Birth" },
                   {nameof(PersonResponse.Gender),"Gender" },
                    {nameof(PersonResponse.Age),"Age" },
                     {nameof(PersonResponse.Country),"Country Name" },
                      {nameof(PersonResponse.Address),"Address" },


            };

            if (parameters != null)
            {
                if (parameters.ContainsKey("SearchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["SearchBy"]);
                }

                if (parameters.ContainsKey("SearchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["SearchString"]);
                }

                if (parameters.ContainsKey("SortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["SortBy"]);
                }

                if (parameters.ContainsKey("SortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["SortOrder"]);
                }
            }
           

        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            ///_logger.LogInformation("PersonsListActionFilter.OnActionExecuting methods --------------");
            _logger.LogInformation("{FilterName}.{MethodName} methods _______________________",nameof(PersonsListActionFilter),nameof(IActionFilter.OnActionExecuting));

            context.HttpContext.Items["arguments"] = context.ActionArguments;


            if (context.ActionArguments.ContainsKey("SearchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["SearchBy"]);
                
                if(!string.IsNullOrEmpty(searchBy))
                {
                    var PersonResonponseList = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Address),
                        nameof(PersonResponse.Age),
                        nameof(PersonResponse.Country),
                        nameof(PersonResponse.ReceiveNewsLetters),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.Gender)

                    };

                if(PersonResonponseList.Any(temp=>temp==searchBy)==false)
                    {
                        

                        _logger.LogInformation("actual value of searchBy is {searchBy}", searchBy);
                        context.ActionArguments["SearchBy"] = nameof(PersonResponse.PersonName);
                        string? updatedvalue = Convert.ToString(context.ActionArguments["SearchBy"]);
                        _logger.LogInformation("Updated valure of searchBy is {updatedvalue}",updatedvalue);




                    }

                }
            }

        }
    }
}
