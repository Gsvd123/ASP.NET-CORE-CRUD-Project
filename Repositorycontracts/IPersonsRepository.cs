using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositorycontracts
{
   public  interface IPersonsRepository
    {
        /// <summary>
        /// Add a Person object to the Person table 
        /// </summary>
        /// <param name="person">person object to add </param>
        /// <returns></returns>
        Task<Person> AddPerson(Person person);


        /// <summary>
        /// Returns list of persons 
        /// </summary>
        /// <returns></returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Return a person object based on the personid , otherwise returns null
        /// </summary>
        /// <param name="personId">personid to check</param>
        /// <returns>person object</returns>
        Task<Person?> GetPersonByPersonID(Guid personId);

        /// <summary>
        /// Returns all person object based on the given expression 
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>Matching persons with given condition </returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Delete the object based on the PersonID
        /// </summary>
        /// <param name="PersonID">Person Id to search </param>
        /// <returns>Return true, if the deletion is successfull;  otherwise false</returns>
        Task<bool> DeletePersonByPersonID(Guid PersonID);

        /// <summary>
        /// Update the person object (person name ans other details) based on the person id 
        /// </summary>
        /// <param name="person">person id to search</param>
        /// <returns>returns updated object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
