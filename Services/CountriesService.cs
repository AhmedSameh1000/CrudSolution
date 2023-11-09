using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContacts;
using ServiceContract.DTOs;
using ServiceContract.Interfaces;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countries;

        public CountriesService(ICountriesRepository countries)
        {
            _countries = countries;
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
            if (await _countries.GetCountryByName(countryForCreateDTO.Name) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryForCreateDTO to Country type
            Country country = countryForCreateDTO.ToCountry();
            //generate Id
            country.Id = Guid.NewGuid();
            //Validation: Every thing is ok , Add country object into _countries
            await _countries.AddCountry(country);

            return country.ToCountryForReturn();
        }

        public async Task<List<CountryForReturnDto>> GetAllCoutries()
        {
            return (await _countries.GetAllCountries()).Select(c => c.ToCountryForReturn()).ToList();
        }

        public async Task<CountryForReturnDto> GetCountryById(Guid? id)
        {
            if (id == null)
                return null;

            Country? Country = await _countries.GetCountryById(id.Value);

            if (Country is null)
                return null;

            return Country.ToCountryForReturn();
        }
    }
}