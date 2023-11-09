using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContacts
{
    public interface ICountriesRepository
    {
        Task<Country> AddCountry(Country country);

        Task<List<Country>> GetAllCountries();

        Task<Country?> GetCountryById(Guid Id);

        Task<Country?> GetCountryByName(string Name);
    }
}