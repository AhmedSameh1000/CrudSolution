using Entities;
using ServiceContract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.DTOs
{
    public class CountryForReturnDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(CountryForReturnDto))
            {
                return false;
            }

            CountryForReturnDto country_to_compare = (CountryForReturnDto)obj;

            return Id == country_to_compare.Id && Name == country_to_compare.Name;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + Id.GetHashCode();
            return hash;
        }
    }

    public static class CountryExtensions
    {
        public static CountryForReturnDto ToCountryForReturn(this Country country)
        {
            return new CountryForReturnDto()
            {
                Id = country.Id,
                Name = country.Name,
            };
        }
    }
}