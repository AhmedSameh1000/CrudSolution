using ServiceContract.DTOs;
using ServiceContract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Interfaces
{
    public interface IPersonService
    {
        Task<PersonForReturnDTO> AddPerson(PersonForCreateDTO? personForCreate);

        Task<List<PersonForReturnDTO>> GetAllPerson();

        Task<PersonForReturnDTO> GetPersonById(Guid id);

        Task<List<PersonForReturnDTO>> GetFilteredPersons(string searchBy, string? searchString);

        List<PersonForReturnDTO> GetSortedPersons(List<PersonForReturnDTO> allPersons, string sortBy, SortOrderOptions sortOrder);

        Task<PersonForReturnDTO> UpdatePerson(PersonForUpdateDTO? personForUpdate);

        Task<bool> RemovePerson(Guid Id);

        Task<MemoryStream> GetPersonsCSV();
    }
}