using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountriesService
    {
        public Task<CountryResponse> AddCountry(CountryAddRequest? countryaddrequest);

        /// <summary>
        /// to get all countries
        /// </summary>
        /// <returns></returns>
        public Task<List<CountryResponse>> GetAllCountries();
        
        /// <summary>
        /// returns country object based on given country Id
        /// </summary>
        /// <param name="countryID">countryID(guid) to search </param>
        /// <returns>Mactching country as countryResponse object </returns>

        public Task<CountryResponse?> GetCountryByCountryID (Guid? countryID);

        /// <summary>
        /// Uploads coutries to the database
        /// </summary>
        /// <param name="excelFile">list of countrires</param>
        /// <returns>return number of countries added</returns>
        public Task<int> UploadCountriesFromExcelFile(IFormFile excelFile);
    }
}
