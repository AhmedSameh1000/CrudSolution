using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly CrudDbContext _context;

        public CountriesRepository(CrudDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Country> AddCountry(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _context.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryById(Guid id)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Country?> GetCountryByName(string name)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}