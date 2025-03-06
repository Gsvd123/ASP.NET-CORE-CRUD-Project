using Entities;
using Microsoft.EntityFrameworkCore;
using Repositorycontracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db; 
        public PersonsRepository(ApplicationDbContext db)
        {
               _db = db;
        }
        public async Task<bool> DeletePersonByPersonID(Guid PersonID)
        {
             _db.Persons.RemoveRange(_db.Persons.Where(temp=>temp.PersonID==PersonID));

            int isDeleted=await _db.SaveChangesAsync();

            return isDeleted > 0;
            
          
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await  _db.Persons.Include("Country").ToListAsync();


        }

        public async  Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
           return  await _db.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person> AddPerson(Person person)
        {
         await _db.Persons.AddAsync(person);
          await  _db.SaveChangesAsync();

            return person;  

        }

        public async Task<Person?> GetPersonByPersonID(Guid personId)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp
                => temp.PersonID==personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
           Person? matching_Person=await _db.Persons.Include("Country").FirstOrDefaultAsync(temp=>temp.PersonID==person.PersonID);

            if(matching_Person==null) {
                return person;
            }

           matching_Person.PersonName = person.PersonName;
            matching_Person.Gender = person.Gender;
            matching_Person.Address = person.Address;
            matching_Person.DateOfBirth = person.DateOfBirth;   
            matching_Person.TIN = person.TIN;
            matching_Person.Country = person.Country;

            int isUpdated=await _db.SaveChangesAsync();

            return matching_Person;

        }
    }
}
