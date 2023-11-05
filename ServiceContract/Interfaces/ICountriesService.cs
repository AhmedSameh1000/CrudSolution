using ServiceContract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Interfaces
{
    public interface ICountriesService
    {
        CountryForReturnDto AddCountry(CountryForCreateDto? country);

        List<CountryForReturnDto> GetAllCoutries();

        CountryForReturnDto GetCountryById(Guid? id);
    }
}