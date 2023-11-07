using CsvHelper;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContract.DTOs;
using ServiceContract.Enums;
using ServiceContract.Interfaces;
using Services.Healpers;
using System.Globalization;

namespace Services
{
    public class PersonsService : IPersonService
    {
        private readonly CrudDbContext _dbContext;
        private readonly ICountriesService _countriesService;

        public PersonsService(CrudDbContext dbContext, ICountriesService countriesService)
        {
            _dbContext = dbContext;
            _countriesService = countriesService;
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
            //

            //add person object to persons list
            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();
            //convert the Person object into PersonForReturnDTO type
            PersonForReturnDTO personForReturnDTO = person.ToPersonForReturn();

            //personForReturnDTO.Country = _countriesService.GetCountryById(person.CountryId)?.Name;
            return personForReturnDTO;
        }

        public async Task<List<PersonForReturnDTO>> GetAllPerson()
        {
            var Persons = await _dbContext.Persons.Include(c => c.Country).Select(c => c.ToPersonForReturn()).ToListAsync();

            return Persons;
        }

        public async Task<PersonForReturnDTO> GetPersonById(Guid id)
        {
            var person = await _dbContext.Persons.Include(c => c.Country).FirstOrDefaultAsync(c => c.Id == id);

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

            switch (searchBy)
            {
                case nameof(Person.Name):
                    matchingPersons = allPersons.Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.Email) && p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(p =>
                    p.DateOfBirth != null && p.DateOfBirth.Value.ToString("dd MM yyyy").StartsWith(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.Gender) && p.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.Country):
                    matchingPersons = allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.Country) && p.Country.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(Person.ReceiveEmails):
                    matchingPersons = allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.ReceiveEmails.ToString()) && p.ReceiveEmails.ToString().StartsWith(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
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

        public async Task<PersonForReturnDTO> UpdatePerson(PersonForUpdateDTO? PersontoUpdate)
        {
            if (PersontoUpdate == null)
                throw new ArgumentNullException(nameof(Person));

            //validation
            Healper.ValidateModel(PersontoUpdate);

            //get matching person object to update
            Person? personForUpdate = await _dbContext.Persons.Include(c => c.Country).FirstOrDefaultAsync(p => p.Id == PersontoUpdate.ID);
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

            _dbContext.Persons.Update(personForUpdate);
            await _dbContext.SaveChangesAsync();
            return personForUpdate.ToPersonForReturn();
        }

        public async Task<bool> RemovePerson(Guid Id)
        {
            if (Id == null)
            {
                throw new ArgumentNullException(nameof(Id));
            }

            Person? person = await _dbContext.Persons.FirstOrDefaultAsync(p => p.Id == Id);
            if (person == null)
                return false;

            _dbContext.Remove(person);
            await _dbContext.SaveChangesAsync();
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

            List<PersonForReturnDTO> persons = _dbContext.Persons
              .Include("Country")
              .Select(p => p.ToPersonForReturn()).ToList();
            await csvWriter.WriteRecordsAsync(persons); //1,abc,abc,.....
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}