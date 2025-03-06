using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorycontracts
{
    public  interface ICountriesRepository
    {
        /// <summary>
        /// Add a new country object to  the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns the country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);


        /// <summary>
        /// Returns all countries in the data store
        /// </summary>
        /// <returns>All countries from the table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on the given country id; otherwise,it returns null
        /// </summary>
        /// <param name="countryID">Country Id to check</param>
        /// <returns></returns>
        Task<Country?> GetCountryByCountryID(Guid countryID);

        /// <summary>
        /// Return country object baseed on the given country Name; otherwise it returns null
        /// </summary>
        /// <param name="CoiuntryName">CountryName to check</param>
        /// <returns></returns>
        Task<Country?> GetCountryByCountryName(string CoiuntryName);
    }
}
