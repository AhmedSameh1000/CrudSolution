using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContacts;
using ServiceContract.DTOs;
using ServiceContract.Enums;
using ServiceContract.Interfaces;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CrudTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonService _personsService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _PersonRepository;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            //before be using repositories when we use dbcontext with services
            //var PersonInitialData = new List<Person>();
            //var CountriesInitialData = new List<Country>();

            ////var DbContext = new Entities.CrudDbContext(new DbContextOptionsBuilder<CrudDbContext>().Options);=>Real DbContext
            //DbContextMock<CrudDbContext> dbContextMock = new(new DbContextOptionsBuilder<CrudDbContext>().Options);//Not Real DbContext||dbContextMock
            //dbContextMock.CreateDbSetMock(dbcontext => dbcontext.Persons, PersonInitialData);
            //dbContextMock.CreateDbSetMock(dbcontext => dbcontext.Countries, CountriesInitialData);
            //var DbContext = dbContextMock.Object;

            //_countriesService = new CountriesService(DbContext);
            //_personsService = new PersonsService(DbContext, _countriesService);

            // now we use Repositories

            _personRepositoryMock = new Mock<IPersonsRepository>();
            _PersonRepository = _personRepositoryMock.Object;
            _personsService = new PersonsService(_PersonRepository);
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        //When we supply null value as PersonForCreateDTO, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonForCreateDTO? personForCreateDTO = null;

            //Act //Using Assert Class
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            // {
            //     await _personsService.AddPerson(personForCreateDTO);
            // });

            var Actual = async () =>
             {
                 await _personsService.AddPerson(personForCreateDTO);
             };

            await Actual.Should().ThrowAsync<ArgumentNullException>();
        }

        //When we supply null value as Person Name, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonForCreateDTO? personForCreateDTO = _fixture.Build<PersonForCreateDTO>()
            .With(c => c.Name, null as string).Create();

            var Expected = personForCreateDTO.ToPerson();

            _personRepositoryMock.Setup(p => p.AddPerson(It.IsAny<Person>())).ReturnsAsync(Expected);
            //Act Using Assert Class
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            // {
            //     await _personsService.AddPerson(personForCreateDTO);
            // });
            var Actual = async () =>
            {
                await _personsService.AddPerson(personForCreateDTO);
            };

            await Actual.Should().ThrowAsync<ArgumentException>();
        }

        //When we supply proper person details, it should insert the person into the persons list; 	and it should return an object of PersonForReturnDTO, which includes with the newly 	generated person id
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            //Create Methode Genrata Rondam Values
            //PersonForCreateDTO? personForCreateDTO = _fixture.Create<PersonForCreateDTO>();

            var PersonForCreate = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").Create();

            var Person = PersonForCreate.ToPerson();
            var expected = Person.ToPersonForReturn();
            _personRepositoryMock.Setup(c => c.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(Person);

            //new PersonForCreateDTO() { Name = "Person 	name...", Email = "person@example.com", CountryId = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveEmails = true };
            //Act

            PersonForReturnDTO personForReturnDTO_from_add = await _personsService.AddPerson(PersonForCreate);

            expected.Id = personForReturnDTO_from_add.Id;
            //List<PersonForReturnDTO> persons_list = await _personsService.GetAllPerson();

            personForReturnDTO_from_add.Id.Should().NotBe(Guid.Empty);
            //Assert.True(personForReturnDTO_from_add.Id != Guid.Empty);
            personForReturnDTO_from_add.Should().BeEquivalentTo(expected);
            //Assert.Contains(personForReturnDTO_from_add, persons_list);
            //persons_list.Should().Contain(personForReturnDTO_from_add);
        }

        #endregion AddPerson

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            var Persons = new List<Person>();

            _personRepositoryMock.Setup(p => p.GetAllPersons())
                .ReturnsAsync(Persons);

            //Act
            List<PersonForReturnDTO> persons_from_get = await _personsService.GetAllPerson();

            //Assert
            //Assert.Empty(persons_from_get);
            persons_from_get.Should().BeEmpty();
        }

        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            //CountryForCreateDto countryForCreate_1 = _fixture.Create<CountryForCreateDto>();
            //CountryForCreateDto countryForCreate_2 = _fixture.Create<CountryForCreateDto>();

            //CountryForReturnDto countryForReturn_1 = await _countriesService.AddCountry(countryForCreate_1);
            //CountryForReturnDto countryForReturn_2 = await _countriesService.AddCountry(countryForCreate_2);

            //PersonForCreateDTO personForCreate_1 = _fixture.Build<PersonForCreateDTO>()
            //    .With(e => e.Email, "Ahmed@gmail.com")
            //    .With(c => c.CountryId, countryForReturn_1.Id).Create();

            //PersonForCreateDTO personForCreate_2 = _fixture.Build<PersonForCreateDTO>()
            //    .With(e => e.Email, "ali@gmail.com")
            //    .With(c => c.CountryId, countryForReturn_2.Id).Create();

            //PersonForCreateDTO personForCreate_3 = _fixture.Build<PersonForCreateDTO>()
            //    .With(e => e.Email, "Sara@gmail.com")
            //    .With(c => c.CountryId, countryForReturn_1.Id).Create();

            //List<PersonForCreateDTO> personForCreate_list = new() { personForCreate_1, personForCreate_2, personForCreate_3 };

            //List<PersonForReturnDTO> personForReturn_list_from_add = new();

            //foreach (PersonForCreateDTO personForCreate in personForCreate_list)
            //{
            //    PersonForReturnDTO personForReturnDTO = await _personsService.AddPerson(personForCreate);
            //    personForReturn_list_from_add.Add(personForReturnDTO);
            //}

            var persons = new List<Person>()
            {
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
            };

            var expectedlist = persons.Select(c => c.ToPersonForReturn()).ToList();

            _personRepositoryMock.Setup(p => p.GetAllPersons())
                .ReturnsAsync(persons);

            //Display expected data
            _testOutputHelper.WriteLine("Expected data:");
            expectedlist.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Act
            List<PersonForReturnDTO> persons_list_from_get = await _personsService.GetAllPerson();
            //Display actual data
            _testOutputHelper.WriteLine("Actual data:");
            persons_list_from_get.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Assert
            foreach (PersonForReturnDTO personForReturnDTO_from_add in persons_list_from_get)
            {
                //Assert.Contains(personForReturnDTO_from_add, persons_list_from_get);
                persons_list_from_get.Should().Contain(personForReturnDTO_from_add);
            }
            persons_list_from_get.Should().BeEquivalentTo(expectedlist);
        }

        #endregion GetAllPersons

        #region GetPersonById

        [Fact]
        public async Task GetPersonById()
        {
            //var CountryforCreate = _fixture.Create<CountryForCreateDto>();

            var Person = _fixture.Build<Person>().With(c => c.Country, null as Country).With(c => c.Email, "Email@gmail.com").Create();

            var expected = Person.ToPersonForReturn();

            _testOutputHelper.WriteLine("Expected Data : ");

            _testOutputHelper.WriteLine(expected.ToString());

            _personRepositoryMock.Setup(p => p.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(Person);

            var Actual = await _personsService.GetPersonById(expected.Id);

            _testOutputHelper.WriteLine("Actual Data : ");

            _testOutputHelper.WriteLine(Actual.ToString());

            //Assert.Equal(expected, Actual);
            Actual.Should().Be(expected);
        }

        #endregion GetPersonById

        #region GetFilteredPersons

        //If the search text is empty and search by is "Name", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            //CountryForCreateDto countryForCreate_1 = _fixture.Create<CountryForCreateDto>();
            //CountryForCreateDto countryForCreate_2 = _fixture.Create<CountryForCreateDto>();

            //CountryForReturnDto countryForReturn_1 = await _countriesService.AddCountry(countryForCreate_1);
            //CountryForReturnDto countryForReturn_2 = await _countriesService.AddCountry(countryForCreate_2);

            //PersonForCreateDTO personForCreate_1 = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").With(c => c.CountryId, countryForReturn_1.Id).Create();

            //PersonForCreateDTO personForCreate_2 = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").With(c => c.CountryId, countryForReturn_1.Id).Create();

            //PersonForCreateDTO personForCreate_3 = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").With(c => c.CountryId, countryForReturn_1.Id).Create();

            //List<PersonForCreateDTO> personForCreate_list = new() { personForCreate_1, personForCreate_2, personForCreate_3 };

            //List<PersonForReturnDTO> personForReturn_list_from_add = new();

            //foreach (PersonForCreateDTO personForCreate in personForCreate_list)
            //{
            //    PersonForReturnDTO personForReturnDTO = await _personsService.AddPerson(personForCreate);
            //    personForReturn_list_from_add.Add(personForReturnDTO);
            //}
            var persons = new List<Person>()
            {
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
            };

            var expectedlist = persons.Select(c => c.ToPersonForReturn()).ToList();

            _personRepositoryMock.Setup(p => p.GetAllPersons())
                .ReturnsAsync(persons);

            _personRepositoryMock.Setup(p => p.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            //Display Expected Data
            _testOutputHelper.WriteLine("Expected:");
            expectedlist.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Act
            List<PersonForReturnDTO> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.Name), "");

            //Display Actual Data
            _testOutputHelper.WriteLine("Actual:");
            persons_list_from_search.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Assert
            //foreach (PersonForReturnDTO personForReturnDTO_from_add in personForReturn_list_from_add)
            //{
            //    //Assert.Contains(personForReturnDTO_from_add, persons_list_from_search);
            //    persons_list_from_search.Should().Contain(personForReturnDTO_from_add);
            //}

            expectedlist.Should().BeEquivalentTo(persons_list_from_search);
        }

        //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            //CountryForCreateDto countryForCreate_1 = _fixture.Create<CountryForCreateDto>();
            //CountryForCreateDto countryForCreate_2 = _fixture.Create<CountryForCreateDto>();

            //CountryForReturnDto countryForReturn_1 = await _countriesService.AddCountry(countryForCreate_1);
            //CountryForReturnDto countryForReturn_2 = await _countriesService.AddCountry(countryForCreate_2);

            //PersonForCreateDTO personForCreate_1 = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").With(c => c.CountryId, countryForReturn_1.Id).Create();

            //PersonForCreateDTO personForCreate_2 = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").With(c => c.CountryId, countryForReturn_1.Id).Create();

            //PersonForCreateDTO personForCreate_3 = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").With(c => c.CountryId, countryForReturn_1.Id).Create();

            //List<PersonForCreateDTO> personForCreate_list = new() { personForCreate_1, personForCreate_2, personForCreate_3 };

            //List<PersonForReturnDTO> personForReturn_list_from_add = new();

            //foreach (PersonForCreateDTO personForCreate in personForCreate_list)
            //{
            //    PersonForReturnDTO personForReturnDTO = await _personsService.AddPerson(personForCreate);
            //    personForReturn_list_from_add.Add(personForReturnDTO);
            //}

            var persons = new List<Person>()
            {
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
            };

            var expectedlist = persons.Select(c => c.ToPersonForReturn()).ToList();

            _personRepositoryMock.Setup(p => p.GetAllPersons())
                .ReturnsAsync(persons);

            _personRepositoryMock.Setup(p => p.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            //Display Expected Data
            _testOutputHelper.WriteLine("Expected:");
            expectedlist.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            //Act
            List<PersonForReturnDTO> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.Name), "am");

            //Display Actual Data
            _testOutputHelper.WriteLine("Actual:");
            persons_list_from_search.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });

            persons_list_from_search.Should().BeEquivalentTo(expectedlist);
            //Assert
            //foreach (PersonForReturnDTO personForReturnDTO_from_add in personForReturn_list_from_add)
            //{
            //    if (personForReturnDTO_from_add.Name != null)
            //    {
            //        if (personForReturnDTO_from_add.Name.StartsWith("am", StringComparison.OrdinalIgnoreCase))
            //        {
            //            //Assert.Contains(personForReturnDTO_from_add, persons_list_from_search);
            //            persons_list_from_search.Should().Contain(personForReturnDTO_from_add);
            //        }
            //    }
            //}
        }

        #endregion GetFilteredPersons

        #region GetSortedPersons

        //When we sort based on Person Name in ASC, it should return persons list in ascending on Person Name
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            //CountryForCreateDto countryForCreate_1 = _fixture.Create<CountryForCreateDto>();
            //CountryForCreateDto countryForCreate_2 = _fixture.Create<CountryForCreateDto>();

            //CountryForReturnDto countryForReturn_1 = await _countriesService.AddCountry(countryForCreate_1);
            //CountryForReturnDto countryForReturn_2 = await _countriesService.AddCountry(countryForCreate_2);

            //PersonForCreateDTO personForCreate_1 = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").With(c => c.CountryId, countryForReturn_1.Id).Create();
            //PersonForCreateDTO personForCreate_2 = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").With(c => c.CountryId, countryForReturn_1.Id).Create();
            //PersonForCreateDTO personForCreate_3 = _fixture.Build<PersonForCreateDTO>().With(c => c.Email, "Email@gmail.com").With(c => c.CountryId, countryForReturn_1.Id).Create();

            //List<PersonForCreateDTO> personForCreate_list = new() { personForCreate_1, personForCreate_2, personForCreate_3 };

            //List<PersonForReturnDTO> personForReturn_list_from_add = new();

            //foreach (PersonForCreateDTO personForCreate in personForCreate_list)
            //{
            //    PersonForReturnDTO personForReturnDTO = await _personsService.AddPerson(personForCreate);
            //    personForReturn_list_from_add.Add(personForReturnDTO);
            //}
            var persons = new List<Person>()
            {
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
                _fixture.Build<Person>().With(c=>c.Country ,null as Country).With(c=>c.Email,"Emil@gmail.com").Create(),
            };

            var expectedlist = persons.Select(c => c.ToPersonForReturn()).ToList();

            //_personRepositoryMock.Setup(p => p.GetAllPersons())
            //    .ReturnsAsync(persons);

            //_personRepositoryMock.Setup(p => p.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            //    .ReturnsAsync(persons);

            //Display Expected Data
            _testOutputHelper.WriteLine("Expected:");
            expectedlist.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });
            _personRepositoryMock.Setup(c => c.GetAllPersons()).ReturnsAsync(persons);

            List<PersonForReturnDTO> allPersons = await _personsService.GetAllPerson();
            //Act
            List<PersonForReturnDTO> persons_list_from_sort = _personsService.GetSortedPersons(allPersons, nameof(Person.Name), SortOrderOptions.ASC);

            //Display Actual Data
            _testOutputHelper.WriteLine("Actual:");
            persons_list_from_sort.ForEach(p =>
            {
                _testOutputHelper.WriteLine(p.ToString());
            });
            //personForReturn_list_from_add = personForReturn_list_from_add.OrderBy(p => p.Name).ToList();
            //Assert
            //for (int i = 0; i < personForReturn_list_from_add.Count; i++)
            //{
            //    //Assert.Equal(personForReturn_list_from_add[i], persons_list_from_sort[i]);

            //    persons_list_from_sort[i].Should().Be(personForReturn_list_from_add[i]);
            //}
            persons_list_from_sort.Should().BeInAscendingOrder(c => c.Name);

            persons_list_from_sort.Should().BeEquivalentTo(expectedlist);
        }

        #endregion GetSortedPersons

        #region UpdatePerson

        //When we supply null as PersonForUpdateDTO, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonForUpdateDTO? personForUpdateDTO = null;

            //Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    //Act
            //    await _personsService.UpdatePerson(personForUpdateDTO);
            //});

            var Actual = async () =>
            {
                //Act
                await _personsService.UpdatePerson(personForUpdateDTO);
            };

            await Actual.Should().ThrowAsync<ArgumentNullException>();
        }

        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonForUpdateDTO? personForUpdateDTO = _fixture.Create<PersonForUpdateDTO>();

            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //   {
            //       //Act
            //       await _personsService.UpdatePerson(personForUpdateDTO);
            //   });

            var actual = async () =>
            {
                //Act
                await _personsService.UpdatePerson(personForUpdateDTO);
            };
            await actual.Should().ThrowAsync<ArgumentException>();
        }

        //Validations
        //When Person Name is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            //CountryForCreateDto countryForCreateDTO = _fixture.Create<CountryForCreateDto>();
            //CountryForReturnDto countryForReturnDTO = await _countriesService.AddCountry(countryForCreateDTO);

            //PersonForCreateDTO personForCreateDTO = _fixture.Build<PersonForCreateDTO>().With(c => c.CountryId, countryForReturnDTO.Id).With(c => c.Email, "Email@gmail.com").Create();

            //PersonForReturnDTO personForReturnDTO_from_add = await _personsService.AddPerson(personForCreateDTO);

            //PersonForUpdateDTO personForUpdateDTO = personForReturnDTO_from_add.ToPersonForUpdateDTO();
            var person = _fixture.Build<Person>()
                .With(c => c.Country, null as Country)
                .With(c => c.Email, "Emil@gmail.com")
                .With(c => c.Gender, "Male")
                .With(c => c.Name, null as string).Create();

            var expcted = person.ToPersonForReturn();

            var PersonForUpdate = expcted.ToPersonForUpdateDTO();
            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    //Act
            //    await _personsService.UpdatePerson(personForUpdateDTO);
            //});

            var Actual = async () =>
            {
                //Act
                await _personsService.UpdatePerson(PersonForUpdate);
            };
            await Actual.Should().ThrowAsync<ArgumentException>();
        }

        //First, add a new person and try to update the person name,email,gender
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdate()
        {
            //Arrange
            //CountryForCreateDto countryForCreateDTO = _fixture.Create<CountryForCreateDto>();
            //CountryForReturnDto countryForReturnDTO = await _countriesService.AddCountry(countryForCreateDTO);

            //PersonForCreateDTO personForCreateDTO = _fixture.Build<PersonForCreateDTO>().With(c => c.CountryId, countryForReturnDTO.Id).With(c => c.Email, "email@gmail.com").Create();

            //PersonForReturnDTO personForReturnDTO_from_add = await _personsService.AddPerson(personForCreateDTO);

            ////Display Person before update
            //_testOutputHelper.WriteLine("Person before update:");
            //_testOutputHelper.WriteLine(personForReturnDTO_from_add.ToString());

            //PersonForUpdateDTO PersonForUpdateDTO = personForReturnDTO_from_add.ToPersonForUpdateDTO();
            //PersonForUpdateDTO.Name = "Amany";
            //PersonForUpdateDTO.Email = "amany@email.com";
            //PersonForUpdateDTO.Gender = GenderOptions.Female;

            ////Act
            //PersonForReturnDTO personForReturnDTO_from_update = await _personsService.UpdatePerson(PersonForUpdateDTO);

            //PersonForReturnDTO? personForReturnDTO_from_get = await _personsService.GetPersonById(personForReturnDTO_from_update.Id);
            var person = _fixture.Build<Person>()
               .With(c => c.Country, null as Country)
               .With(c => c.Email, "Emil@gmail.com")
               .With(c => c.Gender, "Male").Create();

            var expcted = person.ToPersonForReturn();

            var PersonForUpdate = expcted.ToPersonForUpdateDTO();

            _personRepositoryMock.Setup(p => p.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            _personRepositoryMock.Setup(p => p.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Display Person after update
            _testOutputHelper.WriteLine("Person after update:");
            _testOutputHelper.WriteLine(expcted!.ToString());

            var Actual = await _personsService.UpdatePerson(PersonForUpdate);
            //Assert

            //Assert.Equal(personForReturnDTO_from_get, personForReturnDTO_from_update);
            Actual.Should().Be(expcted);
        }

        #endregion UpdatePerson

        #region DeletePerson

        //If you supply an invalid Person Id, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personsService.RemovePerson(Guid.NewGuid());

            //Assert
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }

        //If you supply an valid Person ID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            var person = _fixture.Build<Person>()
                  .With(c => c.Country, null as Country)
                  .With(c => c.Email, "Emil@gmail.com")
                  .With(c => c.Gender, "Male")
                  .With(c => c.Name, null as string).Create();

            _personRepositoryMock.Setup(c => c.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            _personRepositoryMock.Setup(c => c.DeletePersonById(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            //Act
            bool isDeleted = await _personsService.RemovePerson(person.Id);

            //Assert
            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();
        }

        #endregion DeletePerson
    }
}