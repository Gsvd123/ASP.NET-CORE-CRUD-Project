using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Repositorycontracts;
using Serilog;
using SerilogTimings;
using serviceclass.Helpers;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace serviceclass
{
    public class PersonService : ServiceContracts.IPersonService


    {

        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public  PersonService(IPersonsRepository personsRepository,ILogger<PersonService> logger,IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
            
        }



        //private readonly List<Person> _personList;
        // private readonly ICountriesService _countriesService;
        //private readonly ApplicationDbContext _db;

        //public PersonService(ApplicationDbContext personsDbContext, ICountriesService countriesService)
        //{
        //    _db = personsDbContext;
        //    _countriesService = countriesService;

        //}

        //private PersonResponse ConvertPersonToPersonResponse(Person person)
        //{
        //    PersonResponse personResponse = person.ToPersonResponse();
        //    personResponse.Country = _countriesService.GetCountryByCountryID(personResponse.CountryID)?.CountryName;

        //    return personResponse;
        //}

        #region AddPerson

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            //check if personAddrequest is null or not
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            //validate PersonName
            //if (string.IsNullOrEmpty(personAddRequest.PersonName))
            //{
            //    throw new ArgumentException("Person Name  can't be blank");
            //}

            //   //model validation
            ValidationHelper.ModelValidation(personAddRequest);

            //converting personAddRequest to person
            Person person = personAddRequest.ToPerson();

            //Generate the person Id
            person.PersonID = Guid.NewGuid();

            await _personsRepository.AddPerson(person);
            //Add to list<person>
            //await _db.Persons.AddAsync(person);

            //await _db.SaveChangesAsync();

            //Stored Procedure

            //_db.sp_InsertPerson(person);

            //generate personResponse object with personId

            return person.ToPersonResponse();

        }
        #endregion

        #region GetAllPersons
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            List<Person> personsList = await _personsRepository.GetAllPersons();

            return personsList.Select(temp => temp.ToPersonResponse()).ToList();

            //Stored Procedure
            //return _db.sp_GetAllPersons().Select(temp=>ConvertPersonToPersonResponse(temp)).ToList();




            //List<PersonResponse> personList = new List<PersonResponse>();
            //foreach(Person person in _personList)
            //{
            //    personList.Add(person.ToPersonResponse());
            //}
            //return personList;
        }
        #endregion

        #region GetPersonByPersonID
        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                return null;
            }

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            if (person == null)
            {
                return null;
            }

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string SearchBy, string? searchString)
        {
            //List<PersonResponse> allpersons = await GetAllPersons(); // no need to create a instance to call same class


            //if (String.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(SearchBy))
            //{
            //    return MatchedPersons;
            //}
            List<Person> MatchedPersons;

            //In LINQ convertion it won't convert stringComparison, isNullorEmpty . so we can ignore
            using (Operation.Time("Time for Filtered Person from Database................."))
            {
                MatchedPersons = SearchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                    await _personsRepository.GetFilteredPersons(temp => temp.PersonName.Contains(searchString)),

                    nameof(PersonResponse.Email) =>
                    await _personsRepository.GetFilteredPersons(temp => temp.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                    await _personsRepository.GetFilteredPersons(temp => temp.DateOfBirth.Value.ToString("dd mm yyyy").Contains(searchString)),

                    nameof(PersonResponse.Gender) =>
                    await _personsRepository.GetFilteredPersons(temp => temp.Gender.Contains(searchString)),

                    nameof(PersonResponse.CountryID) =>
                    await _personsRepository.GetFilteredPersons(temp => temp.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                    await _personsRepository.GetFilteredPersons(temp => temp.Address.Contains(searchString)),

                    _ => await _personsRepository.GetAllPersons()
                };
            }

            _diagnosticContext.Set("persons", MatchedPersons);
         
            return MatchedPersons.Select(temp=>temp.ToPersonResponse()).ToList();
        }


        #endregion


        #region GetSortedPersons
        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allpersons, string SortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(SortBy)) return allpersons;
            
            List<PersonResponse> SortedPerson_list = (SortBy, sortOrder)
                switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASD) => allpersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASD) => allpersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASD) => allpersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASD) => allpersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASD) => allpersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASD) => allpersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASD) => allpersons.OrderByDescending(temp => temp.ReceiveNewsLetters.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allpersons.OrderByDescending(temp => temp.ReceiveNewsLetters.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),



                _ => allpersons
            };

       
        
            return SortedPerson_list;

            
        }

        #endregion
   

        #region UpdateRegion

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null) throw new ArgumentNullException(nameof(Person));

            //all validation 
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person to upddate
           // Person? Matching_person = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID);

            Person? Matching_person= await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);

                  

            if (Matching_person == null) throw new ArgumentException("Given person id doesn't exsist");

            Matching_person.PersonName = personUpdateRequest.PersonName;
            Matching_person.Email = personUpdateRequest.Email;
            Matching_person.DateOfBirth = personUpdateRequest.DateOfBirth;
            Matching_person.Address = personUpdateRequest.Address;
            Matching_person.Gender = personUpdateRequest?.Gender.ToString();
            Matching_person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            Matching_person.CountryID = personUpdateRequest.CountryID;

            await _personsRepository.UpdatePerson(Matching_person);
            return Matching_person.ToPersonResponse();



        }

        #endregion

        #region DeletePerson
        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null) throw new ArgumentNullException();
            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            if (person == null)
                return false;

            //_db.Persons.Remove(_db.Persons.First(person => person.PersonID == personID));

            //await _db.SaveChangesAsync();

            await _personsRepository.DeletePersonByPersonID(personID.Value);
            return true;
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memorystream = new MemoryStream();
            StreamWriter StreamWriter = new StreamWriter(memorystream);

            //below configuration code for modify or change the order of data

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(StreamWriter, csvConfiguration);

            //CsvWriter csvWriter=new CsvWriter(StreamWriter,CultureInfo.InvariantCulture);


            //csvWriter.WriteHeader<PersonResponse>();

            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));

            csvWriter.NextRecord();

            List<PersonResponse> PersonList = await GetAllPersons();

            //_db.Persons.Include("Country").Select(temp => temp.ToPersonResponse()).ToList();

            foreach (PersonResponse person in PersonList)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                csvWriter.WriteField(person.Age);
                if (person.DateOfBirth.HasValue)
                {
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("dd-MM-yyyy"));
                }
                csvWriter.NextRecord();
                csvWriter.Flush();

            }

            //await csvWriter.WriteRecordsAsync(PersonList);  

            memorystream.Position = 0;

            return memorystream;

        }


        #endregion

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                worksheet.Cells["A1"].Value = "PersonName";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Date of Birth";
                worksheet.Cells["D1"].Value = "Age";
                worksheet.Cells["E1"].Value = "Gender";
                worksheet.Cells["F1"].Value = "Address";
                worksheet.Cells["G1"].Value = "Country";
                worksheet.Cells["H1"].Value = "Receive NewLetters";

                using (ExcelRange headercells = worksheet.Cells["A1:H1"])
                {
                    headercells.Style.Font.Bold = true;
                    headercells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headercells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;

                List<PersonResponse> personsList = await GetAllPersons(); //_db.Persons.Include("Country").Select(temp=>temp.ToPersonResponse()).ToList(); 

                foreach(PersonResponse person in personsList)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                    {
                        worksheet.Cells[row, 3].Value = person.DateOfBirth?.ToString("dd-MM-yyyy");
                    }

                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row,5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.Address;
                    worksheet.Cells[row, 7].Value = person.Country;
                    worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;
                    row++;
                }

                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;

            return memoryStream;


        }

    }
}
