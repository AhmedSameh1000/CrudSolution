using Entities;
using ServiceContract.DTOs;
using ServiceContract.Enums;
using ServiceContract.Interfaces;
using Services.Healpers;

namespace Services
{
    public class PersonsService : IPersonService
    {
        private readonly List<Person> _Persons;
        private readonly ICountriesService _CountriesService;

        public PersonsService()
        {
            _CountriesService = new CountriesService();
            _Persons = new List<Person>();
        }

        public PersonForReturnDTO AddPerson(PersonForCreateDTO? personForCreate)
        {
            if (personForCreate == null)
            {
                throw new ArgumentNullException(nameof(personForCreate));
            }

            //Validate PersonName
            //if (string.IsNullOrEmpty(personForCreate.Name))
            //{
            //    throw new ArgumentException("Person Name can't be blank");
            //}

            //ValidationContext validationContext = new ValidationContext(personForCreate);
            //var ValidationResults = new List<ValidationResult>();

            //bool isvalid = Validator.TryValidateObject(personForCreate, validationContext, ValidationResults, true);

            //if (!isvalid)
            //{
            //    throw new ArgumentException(ValidationResults.FirstOrDefault()?.ErrorMessage);
            //}

            Healper.ValidateModel(personForCreate);
            //convert personForCreate into Person type
            Person person = personForCreate.ToPerson();

            //generate Person
            //
            person.Id = Guid.NewGuid();

            //add person object to persons list
            _Persons.Add(person);

            //convert the Person object into PersonForReturnDTO type
            PersonForReturnDTO personForReturnDTO = person.ToPersonForReturn();
            personForReturnDTO.Country = _CountriesService.GetCountryById(person.CountryId)?.Name;
            return personForReturnDTO;
        }

        public List<PersonForReturnDTO> GetAllPerson()
        {
            return _Persons.Select(c => c.ToPersonForReturn()).ToList();
        }

        public PersonForReturnDTO GetPersonById(Guid id)
        {
            var person = _Persons.FirstOrDefault(c => c.Id == id);

            if (person == null)
                return null;

            return person.ToPersonForReturn();
        }

        public List<PersonForReturnDTO> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonForReturnDTO> allPersons = GetAllPerson();
            List<PersonForReturnDTO> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            switch (searchBy)
            {
                case nameof(Person.Name):
                    matchingPersons = allPersons.Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.Email) && p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(p =>
                    p.DateOfBirth != null && p.DateOfBirth.Value.ToString("dd MM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.Gender) && p.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.CountryId):
                    matchingPersons = allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.Country) && p.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                default: matchingPersons = allPersons; break;
            }
            return matchingPersons;
        }

        public List<PersonForReturnDTO> GetSortedPersons(List<PersonForReturnDTO> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonForReturnDTO> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonForReturnDTO.Name), SortOrderOptions.ASC) => allPersons.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonForReturnDTO.Name), SortOrderOptions.DESC) => allPersons.OrderByDescending(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonForReturnDTO.Email), SortOrderOptions.ASC) => allPersons.OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonForReturnDTO.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonForReturnDTO.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(p => p.DateOfBirth).ToList(),

                (nameof(PersonForReturnDTO.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(p => p.DateOfBirth).ToList(),

                (nameof(PersonForReturnDTO.Age), SortOrderOptions.ASC) => allPersons.OrderBy(p => p.Age).ToList(),

                (nameof(PersonForReturnDTO.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(p => p.Age).ToList(),

                (nameof(PersonForReturnDTO.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonForReturnDTO.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonForReturnDTO.Country), SortOrderOptions.ASC) => allPersons.OrderBy(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonForReturnDTO.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public PersonForReturnDTO UpdatePerson(PersonForUpdateDTO? PersontoUpdate)
        {
            if (PersontoUpdate == null)
                throw new ArgumentNullException(nameof(Person));

            //validation
            Healper.ValidateModel(PersontoUpdate);

            //get matching person object to update
            Person? personForUpdate = _Persons.FirstOrDefault(p => p.Id == PersontoUpdate.ID);
            if (personForUpdate == null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            }

            //update all details
            personForUpdate.Name = PersontoUpdate.Name;
            personForUpdate.Email = PersontoUpdate.Email;
            personForUpdate.DateOfBirth = PersontoUpdate.DateOfBirth;
            personForUpdate.Gender = PersontoUpdate.Gender.ToString();
            personForUpdate.CountryId = PersontoUpdate.CountryId;
            personForUpdate.ReceiveEmails = PersontoUpdate.ReceiveEmails;

            return personForUpdate.ToPersonForReturn();
        }

        public bool RemovePerson(Guid Id)
        {
            if (Id == null)
            {
                throw new ArgumentNullException(nameof(Id));
            }

            Person? person = _Persons.FirstOrDefault(p => p.Id == Id);
            if (person == null)
                return false;

            _Persons.Remove(person);

            return true;
        }
    }
}