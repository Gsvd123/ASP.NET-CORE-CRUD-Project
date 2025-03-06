using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repositorycontracts;
using serviceclass;
using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Test
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            //Commented line is for Application DbContext

            //var CountryInitialData= new List<Country>();
            //    DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            //ApplicationDbContext dbContext = dbContextMock.Object;

            //dbContextMock.CreateDbSetMock(temp => temp.Countries, CountryInitialData);

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesService = new CountryService(_countriesRepository);

            _fixture = new Fixture();

        }

        #region AddCountry

        //countryAddRequest is Null, should return  ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            /// await Assert.ThrowsAsync<ArgumentNullException>(async() => await _countriesService.AddCountry(request));
            /// 
            var action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

           await  action.Should().ThrowAsync<ArgumentNullException>();
        }
        
        //country Name is null, should return ArgumentException

        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest request= _fixture.Build<CountryAddRequest>()
                .With(temp=>temp.CountryName,null as string)
                .Create();


            //Assert
            // await Assert.ThrowsAsync<ArgumentException>(async() => await _countriesService.AddCountry(request));

       

            var action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();

            //ArgumenException means some value missed 
        }

        //Country Name is duplicate,shoule return ArgumentException 
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeNullException()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "USA").Create();
            CountryAddRequest? countryAddRequest1 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "USA").Create();

            //CountryAddRequest countryAddRequest=new CountryAddRequest() { CountryName="USA"};
            //CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };



            Country country=countryAddRequest.ToCountry();

            //Mocking the  AddCountry Method
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(country);

            //Act
            var action = async () =>
            {
                await _countriesService.AddCountry(countryAddRequest);
                await _countriesService.AddCountry(countryAddRequest1);
            };
            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    await _countriesService.AddCountry(countryAddRequest);
            //    await _countriesService.AddCountry(countryAddRequest1);
            //}
            //);

            //Assert
             await action.Should().ThrowAsync<ArgumentException>();



        }

        //check the  country name  added or not
        [Fact]
        public async Task AddCountry_PropercountryDetails_ToBeSuccessful()
        {
            //Arrange
            CountryAddRequest? countryAddrequest = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "USA").Create();

            Country country = countryAddrequest.ToCountry();    
            //Mocking the AddCountry
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
               .ReturnsAsync(country);

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddrequest);

    

            //Assert
            //Assert.True(countryResponse.CountryID != Guid.Empty);

            countryResponse.CountryID.Should().NotBeEmpty();

            

           // Assert.Contains(countryResponse, countries_from_GetAllCountries);

            // we need to test one method for one test case

            //countries_from_GetAllCountries.Should().Contain(countryResponse);

        }

        #endregion

        #region GetAllCountries

        [Fact]
        //list of coutries should be empty bu default(before adding any countries)
        public async Task GetAllCountries_EmptyList_ToBeNull()
        {
            //Arrange
            List<Country> countryList = new List<Country>();
           
            //Mocking the Getallcountries method
            _countriesRepositoryMock.Setup(temp=>temp.GetAllCountries())
                .ReturnsAsync(countryList);

            //Act
            List<CountryResponse> actual_country_response_List =await _countriesService.GetAllCountries();

            //Assert
           // Assert.Empty(actual_country_response_List);

            actual_country_response_List.Should().BeEmpty();


        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries_ToBeSuccessful()
        {
            //Arrange
            List<Country> countryList = new List<Country>
           {
               _fixture.Build<Country>().With(temp=>temp.PersonList,null as List<Person>).Create(),
                _fixture.Build<Country>().With(temp=>temp.PersonList,null as List<Person>).Create(),
                 _fixture.Build<Country>().With(temp=>temp.PersonList,null as List<Person>).Create(),
                  _fixture.Build<Country>().With(temp=>temp.PersonList,null as List<Person>).Create()

           };

            //Act
            List<CountryResponse> countries_list_from_AddCountry = countryList.Select(temp=>temp.ToCountryResponse()).ToList();


            //Mocking the GetAllcountries method
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countryList);

            //read the each element from actual list
            List<CountryResponse> actualCountryResponse_List=await _countriesService.GetAllCountries();

            //foreach(CountryResponse expected_country in countries_list_from_AddCountry)
            //{
            //    Assert.Contains(expected_country, actualCountryResponse_List);
            //}

            actualCountryResponse_List.Should().BeEquivalentTo(countries_list_from_AddCountry);

        }

        #endregion

        #region GetcountryBycountryID

        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            //Arrange
            Guid? countryId = null;

            //Act

            CountryResponse? countryResponse_From_GetMethod = await _countriesService.GetCountryByCountryID(countryId);

            //Assert

            //Assert.Null(countryResponse_From_GetMethod);

            countryResponse_From_GetMethod.Should().BeNull();
        }

        [Fact]
        //it compares the current object to another object of countryResponse type and returns true,if both are same test case will pass;
        public async Task GetCountryByCountryID_validCountryID_ToBeSuccessful()
        {
            //Arrange
            Country country = _fixture.Build<Country>().With(temp => temp.PersonList, null as List<Person>).Create();

            CountryResponse? actual_countryResponse = country.ToCountryResponse();

            //Mocking the GetCountryByCountryID
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            //Act

            CountryResponse? Expected_countryResponse = await _countriesService.GetCountryByCountryID(actual_countryResponse.CountryID);

            //Assert

            //Assert.Equal(actual_countryResponse, Expected_countryResponse);

            actual_countryResponse.Should().Be(Expected_countryResponse);


            #endregion
        }
    }
}
