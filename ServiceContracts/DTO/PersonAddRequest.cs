using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="PersonName can't be blank")]
        
        public string? PersonName { get; set; }

        [Required(ErrorMessage ="Email can't be blank")]
        [EmailAddress(ErrorMessage ="Email should be proper valid one")]
        [DataType(DataType.EmailAddress)]   
        public string? Email { get; set; }

        [DataType(DataType.Date)]

        [Required(ErrorMessage ="Data of Birth can't be blank")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage ="Gender can't be blank")]

        public GenderOptions? Gender { get; set; }

        [Required]

        public Guid? CountryID { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person()
            {
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                Address = Address,
                CountryID = CountryID,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
