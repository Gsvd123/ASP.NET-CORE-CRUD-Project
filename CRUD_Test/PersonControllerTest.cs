using AutoFixture;
using Castle.Core.Logging;
using CRUD_Example.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.Frameworks;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Test
{
    public class PersonControllerTest
    {
        private readonly IPersonService _personService;
        private readonly Mock<IPersonService> _personServiceMock;
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly PersonsController _personsController;
        private readonly Fixture _fixture;

        public PersonControllerTest()
        {
            _personServiceMock = new Mock<IPersonService>();
            _personService = _personServiceMock.Object;

            _countriesServiceMock = new Mock<ICountriesService>();
            _countriesService = _countriesServiceMock.Object;

            var ILoggerMock = new Mock<ILogger<PersonsController>>();


            _fixture = new Fixture();

            _personsController = new PersonsController(_personService, _countriesService,ILoggerMock.Object);
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> PersonResponseList = _fixture.Create<List<PersonResponse>>();

            //Mocking the GetFilteredList
            _personServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(PersonResponseList);


            _personServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(PersonResponseList);

            //Act
            IActionResult ActionResult = await _personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(ActionResult);

            viewResult.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();

            viewResult.ViewData.Model.Should().BeEquivalentTo(PersonResponseList);



        }
        #endregion

        #region Create
        [Fact]
        public async Task Create_IfModelError_ReturnCreateview()
        {
            //Arrange
            PersonAddRequest personAddRequest=_fixture.Create<PersonAddRequest>(); 

            List<CountryResponse> countryResponses =_fixture.Create<List<CountryResponse>>();

            //Mocking the GellcountriesMethod

            _countriesServiceMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countryResponses);

            //Act
            _personsController.ModelState.AddModelError(nameof(PersonResponse.PersonName), "Person can't be Blank");

            IActionResult Result= await _personsController.Create(personAddRequest);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(Result);

            viewResult.ViewData.ModelState.ErrorCount.Should().Be(1);


        }

        [Fact]  
        public async Task Create_IfNotModelError_RedirectToIndexview()
        {
            //Arrange

            PersonAddRequest personAddRequest= _fixture.Create<PersonAddRequest>();

            PersonResponse personResponse=personAddRequest.ToPerson().ToPersonResponse();

            //Mocking the AddPerson method
            _personServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            //Act
            IActionResult Result=await _personsController.Create(personAddRequest);

            //Assert
            
             RedirectToActionResult redirectResult=Assert.IsType<RedirectToActionResult>(Result);

            redirectResult.ActionName.Should().Be("Index");

            redirectResult.ControllerName.Should().Be("Persons");
          
        }

        #endregion

        #region Edit

        [Fact]
        public async Task Edit_PersonNameNotNull_ReturnEditView()
        {
            //Arrange
           PersonResponse personResponse = _fixture.Build<PersonResponse>()
                .With(temp=>temp.Gender,"Male").Create();

           PersonUpdateRequest personUpdateRequest=personResponse.ToPersonUpdateRequest();

           List<CountryResponse> countryList=_fixture.Create<List<CountryResponse>>();  

            //Moking the methods
            _personServiceMock.Setup(temp=>temp.GetPersonByPersonID(personResponse.PersonID)).ReturnsAsync(personResponse); 
            _countriesServiceMock.Setup(temp=>temp.GetAllCountries()).ReturnsAsync(countryList);

            //Act
            IActionResult Result=await _personsController.Edit(personResponse.PersonID);

            //Assert
            ViewResult viewResult=Assert.IsType<ViewResult>(Result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();

            viewResult.ViewData.Model.Should().BeEquivalentTo(personUpdateRequest);

        }

        #endregion
    }
}

