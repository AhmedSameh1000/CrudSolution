using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContract.DTOs;
using ServiceContract.Interfaces;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly CrudDbContext _dbContext;

        public CountriesService(CrudDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CountryForReturnDto> AddCountry(CountryForCreateDto? countryForCreateDTO)
        {
            //Validation: countryForCreateDTO parameter can't be null
            if (countryForCreateDTO == null)
            {
                throw new ArgumentNullException(nameof(countryForCreateDTO));
            }

            //Validation: Name can't be null
            if (countryForCreateDTO.Name == null)
            {
                throw new ArgumentException(nameof(countryForCreateDTO.Name));
            }

            //Validation: Name can't be duplicate
            if (await _dbContext.Countries.AnyAsync(c => c.Name == countryForCreateDTO.Name))
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryForCreateDTO to Country type
            Country country = countryForCreateDTO.ToCountry();

            //Validation: Every thing is ok , Add country object into _countries
            await _dbContext.Countries.AddAsync(country);
            await _dbContext.SaveChangesAsync();
            return country.ToCountryForReturn();
        }

        public async Task<List<CountryForReturnDto>> GetAllCoutries()
        {
            return await _dbContext.Countries.Select(c => c.ToCountryForReturn()).ToListAsync();
        }

        public async Task<CountryForReturnDto> GetCountryById(Guid? id)
        {
            if (id == null)
                return null;

            Country? Country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Id == id.Value);

            if (Country is null)
                return null;

            return Country.ToCountryForReturn();
        }
    }
}