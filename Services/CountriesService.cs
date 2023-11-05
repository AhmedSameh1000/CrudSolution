using Entities;
using ServiceContract.DTOs;
using ServiceContract.Interfaces;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        public readonly List<Country> _Countries;

        public CountriesService()
        {
            _Countries = new List<Country>();
        }

        public CountryForReturnDto AddCountry(CountryForCreateDto? countryForCreateDTO)
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
            if (_Countries.Any(c => c.Name == countryForCreateDTO.Name))
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryForCreateDTO to Country type
            Country country = countryForCreateDTO.ToCountry();

            //generate Id
            country.Id = Guid.NewGuid();

            //Validation: Every thing is ok , Add country object into _countries
            _Countries.Add(country);
            return country.ToCountryForReturn();
        }

        public List<CountryForReturnDto> GetAllCoutries()
        {
            return _Countries.Select(c => c.ToCountryForReturn()).ToList();
        }

        public CountryForReturnDto GetCountryById(Guid? id)
        {
            if (id == null)
                return null;

            Country? Country = _Countries.FirstOrDefault(c => c.Id == id.Value);

            if (Country is null)
                return null;

            return Country.ToCountryForReturn();
        }
    }
}