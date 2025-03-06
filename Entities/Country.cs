using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Entities
{
    //Data 
    public class Country
    {
        [Key]
        public Guid CountryID {  get; set; }    

        public String? CountryName { get; set; } 


        public virtual ICollection<Person> PersonList { get; set; }

    }
}
