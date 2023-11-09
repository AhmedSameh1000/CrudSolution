using CsvHelper;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContacts;
using ServiceContract.DTOs;
using ServiceContract.Enums;
using ServiceContract.Interfaces;
using Services.Healpers;
using System.Globalization;

namespace Services
{
    public class PersonsService : IPersonService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonsService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonForReturnDTO> AddPerson(PersonForCreateDTO? personForCreate)
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
            person.Id = Guid.NewGuid();
            //

            //add person object to persons list
            await _personsRepository.AddPerson(person);

            //convert the Person object into PersonForReturnDTO type
            PersonForReturnDTO personForReturnDTO = person.ToPersonForReturn();

            //personForReturnDTO.Country = _countriesService.GetCountryById(person.CountryId)?.Name;
            return personForReturnDTO;
        }

        public async Task<List<PersonForReturnDTO>> GetAllPerson()
        {
            var Persons = await _personsRepository.GetAllPersons();

            return Persons.Select(c => c.ToPersonForReturn()).ToList();
        }

        public async Task<PersonForReturnDTO> GetPersonById(Guid id)
        {
            var person = await _personsRepository.GetPersonById(id);

            if (person == null)
                return null;

            return person.ToPersonForReturn();
        }

        public async Task<List<PersonForReturnDTO>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonForReturnDTO> allPersons = await GetAllPerson();
            List<PersonForReturnDTO> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            List<Person> persons = searchBy switch
            {
                nameof(Person.Name) => await _personsRepository.GetFilteredPersons(p => p.Name!.StartsWith(searchString)),

                nameof(Person.Email) => await _personsRepository.GetFilteredPersons(p => p.Email!.StartsWith(searchString)),

                nameof(Person.DateOfBirth) => await _personsRepository.GetFilteredPersons(p => p.DateOfBirth!.Value.ToString().Contains(searchString)),

                nameof(Person.Gender) => await _personsRepository.GetFilteredPersons(p => p.Gender!.StartsWith(searchString)),

                nameof(Person.Country) => await _personsRepository.GetFilteredPersons(p => p.Country!.Name!.StartsWith(searchString)),

                _ => await _personsRepository.GetAllPersons(),
            };
            return persons.Select(p => p.ToPersonForReturn()).ToList();
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

        public async Task<PersonForReturnDTO> UpdatePerson(PersonForUpdateDTO? PersontoUpdate)
        {
            if (PersontoUpdate == null)
                throw new ArgumentNullException(nameof(Person));

            //validation
            Healper.ValidateModel(PersontoUpdate);

            //get matching person object to update
            Person? personForUpdate = await _personsRepository.GetPersonById(PersontoUpdate.ID);
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

            await _personsRepository.UpdatePerson(personForUpdate);
            return personForUpdate.ToPersonForReturn();
        }

        public async Task<bool> RemovePerson(Guid Id)
        {
            if (Id == null)
            {
                throw new ArgumentNullException(nameof(Id));
            }
            var Person = await _personsRepository.GetPersonById(Id);

            if (Person == null)
            {
                return false;
            }

            await _personsRepository.DeletePersonById(Id);

            return true;
        }

        //first step download CSV Healper
        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new();
            StreamWriter streamWriter = new(memoryStream);
            CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);

            csvWriter.WriteHeader<PersonForReturnDTO>(); //Id,Name,Email,.....
            csvWriter.NextRecord();

            var persons = _personsRepository.GetAllPersons();
            await csvWriter.WriteRecordsAsync(persons.Result); //1,abc,abc,.....
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}