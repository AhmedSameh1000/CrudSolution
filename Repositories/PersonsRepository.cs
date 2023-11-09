using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContacts;
using System.Linq.Expressions;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly CrudDbContext _context;

        public PersonsRepository(CrudDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonById(Guid id)
        {
            Person? person = _context.Persons.FirstOrDefault(p => p.Id == id);
            if (person == null)
                return false;
            _context.Persons.Remove(person);
            int rowsDeleted = await _context.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _context.Persons.Include(c => c.Country).ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _context.Persons.Include(c => c.Country)
             .Where(predicate)
             .ToListAsync();
        }

        public async Task<Person?> GetPersonById(Guid id)
        {
            return await _context.Persons.Include(c => c.Country)
             .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? personForUpdate = await _context.Persons.FirstOrDefaultAsync(p => p.Id == person.Id);

            if (personForUpdate == null)
                return person;

            personForUpdate.Name = person.Name;
            personForUpdate.Email = person.Email;
            personForUpdate.DateOfBirth = person.DateOfBirth;
            personForUpdate.Gender = person.Gender!.ToString();
            personForUpdate.CountryId = person.CountryId;
            personForUpdate.ReceiveEmails = person.ReceiveEmails;

            await _context.SaveChangesAsync();
            return personForUpdate;
        }
    }
}