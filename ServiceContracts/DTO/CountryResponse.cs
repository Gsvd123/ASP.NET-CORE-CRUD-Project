using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
   public class CountryResponse
    {

        public Guid CountryID { get; set; }
        public String CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(CountryResponse))
            {
                return true;
            }

            CountryResponse country_to_compare= (CountryResponse)obj;

            return CountryID==country_to_compare.CountryID && CountryName==country_to_compare.CountryName;    
        }
    }
    public static class CountryExtension
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryName = country.CountryName,
                CountryID = country.CountryID
            };
        }
    }
}
