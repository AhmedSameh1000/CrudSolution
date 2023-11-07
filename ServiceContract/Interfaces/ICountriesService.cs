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
        Task<CountryForReturnDto> AddCountry(CountryForCreateDto? country);

        Task<List<CountryForReturnDto>> GetAllCoutries();

        Task<CountryForReturnDto> GetCountryById(Guid? id);
    }
}