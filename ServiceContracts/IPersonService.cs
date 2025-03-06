using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonService
    {
        public Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

        public Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// return person object based on given person id
        /// </summary>
        /// <param name="personID"></param>
        /// <returns> return person object</returns>
        public Task<PersonResponse> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Returns all the matched person objects based on given search field and search string
        /// </summary>
        /// <param name="SearchBy"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>

        public Task<List<PersonResponse>> GetFilteredPersons(string SearchBy, string? searchString);
        /// <summary>
        /// returnd sorted persons lisst
        /// </summary>
        /// <param name="allpersons">return sorted all persons</param>
        /// <param name="SortBy">name of the properties </param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns></returns>
        public Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allpersons,string SortBy,SortOrderOptions sortOrder);
        /// <summary>
        /// Update the specified details based on the PersonID
        /// </summary>
        /// <param name="personUpdateRequest">To update Person Details </param>
        /// <returns>returns updated person response object</returns>

        public Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
        /// <summary>
        /// Delete the person object based on the given personId
        /// </summary>
        /// <param name="personID">to delete the person id</param>
        /// <returns>if person object is deleter return true otherwise false</returns>
        
        public Task<bool> DeletePerson(Guid? personID);

        /// <summary>
        /// Returns Persons as CSV
        /// </summary>
        /// <returns>Returns the memory stream with CSV data</returns>
        public Task<MemoryStream> GetPersonsCSV();
        /// <summary>
        /// Returns Persons as Excel
        /// </summary>
        /// <returns>Returns the memory stream with Excel</returns>

        public Task<MemoryStream> GetPersonsExcel();
    }
}
