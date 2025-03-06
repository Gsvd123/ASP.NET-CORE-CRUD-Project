using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Repositorycontracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace serviceclass
{
    public class CountryService : ICountriesService
    {
        // private readonly List<Country> _countries;

        // private readonly ApplicationDbContext _db;

        private readonly ICountriesRepository _countriesRepository;

        public CountryService(ICountriesRepository countriesRepository) { 

          _countriesRepository = countriesRepository;
            
        }
        #region Addcountry
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryaddrequest)
        {

            //return exception if countryaddrequest object is null
            if(countryaddrequest == null)
            {
                throw new ArgumentNullException(nameof(countryaddrequest));
            }

            //return exception if countryName is null
            if(countryaddrequest.CountryName == null) { 
            throw new ArgumentException(nameof(countryaddrequest.CountryName));
             }

            //return exception if countryname is duplicate
            if (await _countriesRepository.GetCountryByCountryName(countryaddrequest.CountryName)!=null) //if name is already there na it will return count is 1 so it is true
            {
                throw new ArgumentException("Given country name is already exist");
            }
            //convert country object to country type
            Country country = countryaddrequest.ToCountry();

            //create countryid and add it

            country.CountryID = Guid.NewGuid();

            //add country to _countries list

            await _countriesRepository.AddCountry(country);
           // await _db.Countries.AddAsync(country);

            //await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }



        #endregion

        #region GetAllCountries
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            
          
            //return await _db.Countries.Select(country=>country.ToCountryResponse()).ToListAsync();

            var list=await _countriesRepository.GetAllCountries();

            return  list.Select(Country=>Country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if(countryID == null) {

                return null;
            }

            Country? _countriesList = await _countriesRepository.GetCountryByCountryID(countryID.Value); // if not available return null

            if(_countriesList == null)
            {
                return null;
            }

            return _countriesList.ToCountryResponse();

        }

        #endregion
        public async  Task<int> UploadCountriesFromExcelFile(IFormFile excelFile)
        {
           MemoryStream memoryStream = new MemoryStream();  
           await excelFile.CopyToAsync(memoryStream);

            int countriesAdded = 0;

            using(ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["CountriesName"];

                int rowCount = worksheet.Dimension.Rows;

                for(int row=2; row<=rowCount; row++) {

                    string? cellvalue = Convert.ToString(worksheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellvalue)) {
                        string countryName = cellvalue;

                        if(await _countriesRepository.GetCountryByCountryName(countryName)==null) {

                            Country country = new Country()
                            {
                                CountryName = countryName,
                            };

                           await  _countriesRepository.AddCountry(country);   

                            //_db.Countries.Add(country);
                            //await _db.SaveChangesAsync();

                            countriesAdded++;
                        }

                    }

                
                }

                return countriesAdded;  
                 
            }
        }


    }
}
