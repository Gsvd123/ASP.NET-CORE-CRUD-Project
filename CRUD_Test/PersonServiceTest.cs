using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Repositorycontracts;
using Serilog;
using serviceclass;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CRUD_Test
{
    public class PersonServiceTest
    {
        private readonly ServiceContracts.IPersonService _PersonService;
        private readonly ICountriesService _CountriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly Mock<IPersonsRepository> _PersonRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly IFixture _fixture;


        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            //commented lines for Application DbContext

            //var CountryInitialData = new List<Country>(); // this is for initial data added
            //var PersonsInitialData =new List<Person>(); 

            //DbContextMock<ApplicationDbContext> dbContextMock= new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            //ApplicationDbContext dbContext = dbContextMock.Object;

    
            //dbContextMock.CreateDbSetMock(temp => temp.Countries);
            //dbContextMock.CreateDbSetMock(temp => temp.Persons);

            _PersonRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _PersonRepositoryMock.Object;



           var ILoggerMock=new Mock<ILogger<PersonService>>();
            var IDiagnosticMock=new Mock<IDiagnosticContext>();
          

            //_CountriesService = new CountryService(null);
            _PersonService = new PersonService(_personsRepository,ILoggerMock.Object,IDiagnosticMock.Object);

          
            
            _testOutputHelper = testOutputHelper;

            _fixture = new Fixture();
        }
        public async Task<List<PersonResponse>> PersonResponse_Expected()
        {
            //Arrange
            CountryResponse countryResponse1 =await  _CountriesService.AddCountry(
                new CountryAddRequest()
                {
                    CountryName = "USA"
                });
            CountryResponse countryResponse2 = await _CountriesService.AddCountry(
                new CountryAddRequest()
                {
                    CountryName = "India"
                });
            CountryResponse countryResponse3 = await  _CountriesService.AddCountry(
                new CountryAddRequest()
                {
                    CountryName = "London"
                });
            List<PersonResponse> PersonResponse_list_Expected = new List<PersonResponse>();
            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                    PersonName="Gokulnath", Email="sample@gmail.com",DateOfBirth=DateTime.Parse("2001-02-01"),CountryID=countryResponse1.CountryID,Gender=GenderOptions.Male,ReceiveNewsLetters=true
                },
                new PersonAddRequest()
                {
                    PersonName="Hemalatha", Email="sample@gmail.com",DateOfBirth=DateTime.Parse("2000-02-01"),CountryID=countryResponse2.CountryID,Gender=GenderOptions.Female,ReceiveNewsLetters=true
                },
                new PersonAddRequest()
                {
                    PersonName="varshini", Email="sample@gmail.com",DateOfBirth=DateTime.Parse("2004-02-01"),CountryID=countryResponse3.CountryID,Gender=GenderOptions.Female,ReceiveNewsLetters=true
                }
            };
            foreach (PersonAddRequest personAddRequest in personAddRequests_list)
            {
                PersonResponse_list_Expected.Add(await _PersonService.AddPerson(personAddRequest));
            }

            return PersonResponse_list_Expected;

        }
        #region AddPerson

        //when we pass null value as AddPersonRequest,it should return ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            PersonAddRequest? addRequest = null;
            //act & assert
           //await  Assert.ThrowsAsync<ArgumentNullException>(async () =>
           // {
           //     await _PersonService.AddPerson(addRequest);
           // });
            //act
            Func<Task> action= async () =>
            {
                await _PersonService.AddPerson(addRequest);
            };

            //assert
           await action.Should().ThrowAsync<ArgumentNullException>();


        }

        //when we pass null value as PersonName , it should return ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            PersonAddRequest addRequest = _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.PersonName,null as string)
                .Create();

            //act
            Func<Task> action = async () =>
            {
                await _PersonService.AddPerson(addRequest);
            };

            //assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        /*when we pass the person details into the personlist ,
         person should add inthe list and it should return object of personResponse,which includes newly creared PersonID */

        [Fact]
        public async Task AddPerson_ProperPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest> ()
                .With(temp=>temp.Email,"Sample@gmail.com")
                          .Create();

            Person person=personAddRequest.ToPerson();

            PersonResponse personResponse_Expected=person.ToPersonResponse();

            //Mocked AddPerson Method

            _PersonRepositoryMock.Setup(temp=>temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            

            PersonResponse personResponse_Actual = await  _PersonService.AddPerson(personAddRequest);
            
           
            //Guid not create in PersonAddRequest so we assign the personID to Expected data
           personResponse_Expected.PersonID=personResponse_Actual.PersonID;

            personResponse_Actual.Should().Be(personResponse_Expected);

           

           // Assert.True(personResponse.PersonID != Guid.Empty);

            personResponse_Actual.PersonID.Should().NotBeEmpty();

            //Assert.Contains(personResponse, personsList);
           // personsList.Should().Contain(personResponse);

          //  Assert.True(personResponse.Email != string.Empty);
         
            personResponse_Actual.Email.Should().NotBeEmpty();

        }
        #endregion

        #region GetPersonByPersonID
        //if we pass null valuea as personId it null as PersonResoponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange
            Guid? personID = null;
            //Act
            PersonResponse personResponse = await _PersonService.GetPersonByPersonID(personID);
            //Assert
            // Assert.Null(personResponse);

            personResponse.Should().BeNull();
        }

        //if we pass proper personID , it should return person response object
        [Fact]
        public async Task GetPersonID_WithPersonID_ToBeSuccessful()
        {
            //Arrange
            //creating countryId

            //In Best Practice of Unit Test is  we need to call only one method for one test case
            //CountryResponse countryResponse = await _CountriesService.AddCountry(
            //  _fixture.Create<CountryAddRequest>()
            //);

            //Act
            Person person= 
              _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample@gmail.com")
                .With(temp=>temp.ReceiveNewsLetters,true)
                .With(temp=>temp.Country,null as Country)
                .Create();

            _testOutputHelper.WriteLine(person.ToString());
            

            PersonResponse PersonResponse_Expected=person.ToPersonResponse();   

            //Mock
            _PersonRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                 .ReturnsAsync(person);

            PersonResponse Person_Response_Actual = await _PersonService.GetPersonByPersonID(person.PersonID);

            //Assert
            //Assert.Equal(PersonResponse_Expected, Person_Response_Actual);
            Person_Response_Actual.Should().Be(PersonResponse_Expected);

        }

        #endregion


        #region GetAllPerson
        [Fact]
        //GetAllPersons() should return an empty list by default
        public async Task GetAllPerson_EmptyList()
        {
            //Arrange
            List<Person> personList = new List<Person>();
            
            //Mock Repository
            _PersonRepositoryMock.Setup(temp => temp.GetAllPersons())
                 .ReturnsAsync(personList);
            
            //Act 
            List<PersonResponse> personResponses_list = await _PersonService.GetAllPersons();

            //Arrange
            //Assert.Empty(personResponses_list);

            personResponses_list.Should().BeEmpty();
        }

        //first we add few persons, and then when we call GetAllPersons(), it should return the same person thath were added
        [Fact]
        public async Task GetAllPerson_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> personList = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample1@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample2@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create(),

                  _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample12@gmail.com")
                .With(temp=>temp.ReceiveNewsLetters,true)
                .With(temp=>temp.Country,null as Country)
                .Create(),

                   _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample3@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create()

            };



            List<PersonResponse> personResponse_list_Expected = personList.Select(temp=>temp.ToPersonResponse()).ToList();  


         
            //Print PersonResponse_list_Expected 

            _testOutputHelper.WriteLine("Expected : ");
            foreach (var response_list in personResponse_list_Expected)
            {
                _testOutputHelper.WriteLine(response_list.ToString());
            }


            //Mocking the mehthod
            _PersonRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(personList);

            List<PersonResponse> personResponse_list_Actual = await _PersonService.GetAllPersons();

            //Print PersonResponse_list_Actual

            _testOutputHelper.WriteLine("Actual : ");
            foreach (PersonResponse response_list in personResponse_list_Actual)
            {
                _testOutputHelper.WriteLine(response_list.ToString());
            }

            //Assert

            //foreach (var personResponse in personResponse_list_Actual)
            //{
            //    Assert.Contains(personResponse, personResponse_list_Expected);
            //}

            personResponse_list_Actual.Should().BeEquivalentTo(personResponse_list_Expected);
        }



        #endregion


        #region GetFilteredPersons
        //if serach string is empty and search by is personName , it should return all persons object
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> personList = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample1@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample2@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create(),

                  _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample12@gmail.com")
                .With(temp=>temp.ReceiveNewsLetters,true)
                .With(temp=>temp.Country,null as Country)
                .Create(),

                   _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample3@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create()

            };



            List<PersonResponse> PersonResponse_list_Expected = personList.Select(temp => temp.ToPersonResponse()).ToList();


            _testOutputHelper.WriteLine("Actual :");
            foreach (var personResponse in PersonResponse_list_Expected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Mocking the GetFilteredPersons method & 
            _PersonRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(personList);

            _PersonRepositoryMock.Setup(temp=>temp.GetAllPersons())
                  .ReturnsAsync(personList);
                


            //Act
            List<PersonResponse> personResponses_list_from_search = await _PersonService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Expected :");
            foreach (var personResponse in personResponses_list_from_search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            //foreach (PersonResponse personResponse in PersonResponse_list_Expected)
            //{
            //    Assert.Contains(personResponse, personResponses_list_from_search);

            //}

            personResponses_list_from_search.Should().BeEquivalentTo(PersonResponse_list_Expected);


        }

        //if pass searchBy is personName and searchString is ma, should return filtered persons
        [Fact]
        public async Task GetFilteredPersons_SearchPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> personList = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample1@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample2@gmail.com")
                
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create(),

                  _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample12@gmail.com")
                .With(temp=>temp.ReceiveNewsLetters,true)
                .With(temp=>temp.Country,null as Country)
                .Create(),

                   _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample3@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create()

            };



            List<PersonResponse> PersonResponse_list_Expected = personList.Select(temp => temp.ToPersonResponse()).ToList();

          

            _testOutputHelper.WriteLine("Actual :");
            foreach (var personResponse in PersonResponse_list_Expected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Mocking the GetFiltered Method 
            _PersonRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(personList);

            _PersonRepositoryMock.Setup(temp=>temp.GetAllPersons())
                .ReturnsAsync(personList);

            //Act
            List<PersonResponse> personResponses_list_from_search = await _PersonService.GetFilteredPersons(nameof(Person.PersonName), "th");

            _testOutputHelper.WriteLine("Expected :");
            foreach (var personResponse in personResponses_list_from_search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            //foreach (PersonResponse personResponse in PersonResponse_list_Expected)
            //{
            //    if (personResponse.PersonName != null)
            //    {
            //        if (personResponse.PersonName.Contains("th", StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(personResponse, personResponses_list_from_search);
            //        }
            //    }


            //}

            //personResponses_list_from_search.Should().OnlyContain(temp=>temp.PersonName.Contains("th",StringComparison.OrdinalIgnoreCase));

            personResponses_list_from_search.Should().BeEquivalentTo(PersonResponse_list_Expected);
        }

        #endregion


        #region GetSortedPersons
        //when we sort based on PersonName in DESC,it should return person list in decending on personName
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> personList = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample1@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample2@gmail.com")

                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create(),

                  _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample12@gmail.com")
                .With(temp=>temp.ReceiveNewsLetters,true)
                .With(temp=>temp.Country,null as Country)
                .Create(),

                   _fixture.Build<Person>()
                .With(temp => temp.Email, "Sample3@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.ReceiveNewsLetters,true)
                .Create()

            };



            List<PersonResponse> PersonResponse_list_Expected = personList.Select(temp => temp.ToPersonResponse()).ToList();

           

            _testOutputHelper.WriteLine("Actual :");

            //Descending the list by personName

             PersonResponse_list_Expected=PersonResponse_list_Expected.OrderByDescending(x => x.PersonName).ToList();

            foreach (var personResponse in PersonResponse_list_Expected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Mocking the GetAllPerson method
            _PersonRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(personList);

            List<PersonResponse> allpersonlist=await _PersonService.GetAllPersons();

            //Act
            List<PersonResponse> personResponses_list_from_sort =await _PersonService.GetSortedPersons(allpersonlist,nameof(Person.PersonName),SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Expected :");
            foreach (var personResponse in personResponses_list_from_sort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            //for(int i= 0;i < personResponses_list_from_sort.Count; i++){

            //    Assert.Equal(PersonResponse_list_Expected[i], personResponses_list_from_sort[i]);
            //}

            personResponses_list_from_sort.Should().BeInDescendingOrder(temp=>temp.PersonName);


        }


        #endregion


        #region UpdatePerson
        [Fact]
        //when we supply null value as  PersonUpdateRequest , it should return ArgumentNullException 
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            ////Assert 
            //await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            //    //Act
            //    await _PersonService.UpdatePerson(personUpdateRequest)
            //);

            //act
            Func<Task> action = async () =>
            {
                await _PersonService.UpdatePerson(personUpdateRequest);
            };

            //assert
            await action.Should().ThrowAsync<ArgumentNullException>();


        }

        //when you supplied invalid ID, it should return AegumentException 
        [Fact]
        public async Task UpdatePerson_InvalidPerson_ToBeArgumentException() {
            //Arrange 
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .With(temp=>temp.Email,"Sample@gmail.com")
                .With(temp=>temp.PersonID,Guid.Empty)
                .Create();


            ////Assert
            //await  Assert.ThrowsAsync<ArgumentException>(async () =>
            //   await _PersonService.UpdatePerson(personUpdateRequest)

            //);

            //act
            Func<Task> action = async () =>
            {
                await _PersonService.UpdatePerson(personUpdateRequest);
            };

            //assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //when we supplied null value as personName, it should return ArgumentException 

        [Fact]
        public async Task UpdatePerson_PersonNamenulll_ToBeArgumentException() {

            //Arrange
            Person person=_fixture.Build<Person>()
                .With(temp=>temp.ReceiveNewsLetters,true)
                .With(temp=>temp.PersonName,null as string)
                .With(temp=>temp.Email,"Sample1@gmail.com")
                .With(temp=>temp.Country,null as Country)
                .With(temp=>temp.Gender,"Male")
                .Create();


         PersonResponse personResponse_Orginal=person.ToPersonResponse();
           

            //Act
           PersonUpdateRequest personUpdateRequest=personResponse_Orginal.ToPersonUpdateRequest();
            

            //Assert

            //await Assert.ThrowsAsync<ArgumentException>(async() =>
            //      await _PersonService.UpdatePerson(personUpdateRequest)
            //  );

            //act
            Func<Task> action = async () =>
            {
                await _PersonService.UpdatePerson(personUpdateRequest);
            };

            //assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        //First add a new person and try to update the  person Name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdate()
        {
            //Arrange

            Person person = _fixture.Build<Person>()
              .With(temp => temp.ReceiveNewsLetters, true)
              .With(temp => temp.PersonName, "Gokulnath")
              .With(temp => temp.Email, "Sample1@gmail.com")
              .With(temp => temp.Country, null as Country)
              .With(temp => temp.Gender, "Male")
              .Create();


            PersonResponse personResponse = person.ToPersonResponse();


            PersonUpdateRequest personUpdateRequest_Expected = personResponse.ToPersonUpdateRequest();
            

            _PersonRepositoryMock.Setup(temp=>temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            _PersonRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse personUpdated = await  _PersonService.UpdatePerson(personUpdateRequest_Expected);

            _testOutputHelper.WriteLine("Expected : ");
            _testOutputHelper.WriteLine(personResponse.ToString());

            PersonResponse personUpdated_Actual = await  _PersonService.GetPersonByPersonID(personUpdated.PersonID);
            _testOutputHelper.WriteLine("Actual: ");
            _testOutputHelper.WriteLine(personUpdated_Actual.ToString());

            //Assert
            //Assert.Equal(personUpdated_Actual, personUpdated);
            personUpdated_Actual.Should().Be(personUpdated);

                }
        #endregion


        #region DeletePerson

        // if pass valid person ID, return true and delete that personobject
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.ReceiveNewsLetters, true)
             .With(temp => temp.PersonName, "Gokulnath")
             .With(temp => temp.Email, "Sample1@gmail.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse personResponse_Orginal = person.ToPersonResponse();

            //Mocking the methods
            _PersonRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _PersonRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            

            //Act
            bool isTrue = await  _PersonService.DeletePerson(personResponse_Orginal.PersonID);

            //Assert
            //Assert.True(isTrue);

            isTrue.Should().BeTrue();
        }


        //if pass invalid person ID, return false

        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isFalse=await _PersonService.DeletePerson(Guid.NewGuid());

            //Assert
            //Assert.False(isFalse);

            isFalse.Should().BeFalse();
        }
        #endregion

    }
}
