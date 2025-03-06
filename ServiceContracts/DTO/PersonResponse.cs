using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }

        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public Guid? CountryID { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public double? Age { get; set; }
        public string Country { get; set; }

        public override bool Equals(object? obj)
        {
           if(obj == null)
            {
                return false;
            }

           if(obj.GetType() != typeof(PersonResponse)) { return false; }

           PersonResponse personResponse = (PersonResponse)obj;

            return personResponse.PersonID == PersonID && personResponse.PersonName == PersonName && personResponse.Email == Email && personResponse.ReceiveNewsLetters == ReceiveNewsLetters && personResponse.DateOfBirth == DateOfBirth && personResponse.CountryID == CountryID && personResponse.Address == Address && personResponse.ReceiveNewsLetters == ReceiveNewsLetters;
        }


        public override string ToString()
        {
            return $"PersonID : {PersonID} , PersonName : {PersonName} , CountryID : {CountryID} ,CountryName : {Country} , DateOfBirth : {DateOfBirth?.ToString("yyyy-mm-dd")} , ReceiveNewsLetter : {ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,PersonName = PersonName,Email = Email,Address = Address,CountryID=CountryID,DateOfBirth=DateOfBirth,
                ReceiveNewsLetters = ReceiveNewsLetters,Gender=(GenderOptions)Enum.Parse(typeof(GenderOptions),Gender,true)
            };
        }
    }

    public static class PersonExtension
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                Country = person.Country?.CountryName
            };
        }
    }
}
