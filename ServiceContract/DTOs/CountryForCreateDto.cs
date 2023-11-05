using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.DTOs
{
    public class CountryForCreateDto
    {
        public string? Name { get; set; }

        public Country ToCountry() => new Country()
        {
            Name = Name,
        };
    }
}