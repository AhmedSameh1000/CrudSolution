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
        PersonForReturnDTO AddPerson(PersonForCreateDTO? personForCreate);

        List<PersonForReturnDTO> GetAllPerson();

        PersonForReturnDTO GetPersonById(Guid id);

        List<PersonForReturnDTO> GetFilteredPersons(string searchBy, string? searchString);

        List<PersonForReturnDTO> GetSortedPersons(List<PersonForReturnDTO> allPersons, string sortBy, SortOrderOptions sortOrder);

        PersonForReturnDTO UpdatePerson(PersonForUpdateDTO? personForUpdate);

        bool RemovePerson(Guid Id);
    }
}