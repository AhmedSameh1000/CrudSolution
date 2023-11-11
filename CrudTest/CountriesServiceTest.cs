using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContacts;
using ServiceContract.DTOs;
using ServiceContract.Interfaces;
using Services;

namespace CrudTest
{
    public class CountriesServiceTest
    {
        private readonly Mock<ICountriesRepository> _CountriesRepositoryMock;
        private readonly ICountriesRepository _CountriesRepository;
        private readonly IFixture _Fixture;
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            //var CountriesInitialData = new List<Country>();
            ////var DbContext = new Entities.CrudDbContext(new DbContextOptionsBuilder<CrudDbContext>().Options);=>Real DbContext
            //DbContextMock<CrudDbContext> dbContextMock = new(new DbContextOptionsBuilder<CrudDbContext>().Options);//Not Real DbContext||dbContextMock

            //dbContextMock.CreateDbSetMock(dbcontext => dbcontext.Countries, CountriesInitialData);

            _CountriesRepositoryMock = new Mock<ICountriesRepository>();
            _CountriesRepository = _CountriesRepositoryMock.Object;
            _Fixture = new Fixture();
            _countriesService = new CountriesService(_CountriesRepository);//Not Real DbContext
        }

        #region AddCountry

        //When CountryForCreateDto is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryForCreateDto? countryForCreateDTO = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
             {
                 //Act
                 await _countriesService.AddCountry(countryForCreateDTO);
             });
        }

        //When the Name is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_NameIsNull()
        {
            //Arrange
            CountryForCreateDto? countryForCreateDTO = new CountryForCreateDto() { Name = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(countryForCreateDTO);
            });
        }

        //When the Name is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateName()
        {
            //Arrange
            CountryForCreateDto? countryForCreateDTO1 = new CountryForCreateDto() { Name = "Egypt" };
            CountryForCreateDto? countryForCreateDTO2 = new CountryForCreateDto() { Name = "Egypt" };

            _CountriesRepositoryMock.Setup(c => c.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(countryForCreateDTO1.ToCountry());

            _CountriesRepositoryMock.Setup(c => c.GetCountryByName(It.IsAny<string>()))
                .ReturnsAsync(countryForCreateDTO2.ToCountry());

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
             {
                 //Act
                 await _countriesService.AddCountry(countryForCreateDTO1);
                 await _countriesService.AddCountry(countryForCreateDTO2);
             });
            var Actual = async () =>
            {
                //Act
                await _countriesService.AddCountry(countryForCreateDTO1);
                await _countriesService.AddCountry(countryForCreateDTO2);
            };
        }

        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryForCreateDto? countryForCreateDTO = new CountryForCreateDto() { Name = "Palestine" };

            //Act
            CountryForReturnDto countryForReturnDTO = await _countriesService.AddCountry(countryForCreateDTO);

            //Assert
            Assert.True(countryForReturnDTO.Id != Guid.Empty);

            //actual_CountryForReturnDto_List.ForEach(ac => Assert.Equivalent(ac, countryForReturnDTO));
        }

        #endregion AddCountry

        #region GetAllCountries

        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Act
            var list = new List<Country>();
            _CountriesRepositoryMock.Setup(c => c.GetAllCountries()).ReturnsAsync(list);

            List<CountryForReturnDto> actualCountryForReturnList = await _countriesService.GetAllCoutries();

            //Assert
            actualCountryForReturnList.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<Country> Countries = new() {
                    new(){ Name = "Iraq" },
                    new(){ Name = "Syria" }};

            //Act  expected CountryForReturnDto List
            List<CountryForReturnDto> expected_countryForReturnDTO_List = Countries.Select(c => c.ToCountryForReturn()).ToList();

            _CountriesRepositoryMock.Setup(c => c.GetAllCountries()).ReturnsAsync(Countries);
            List<CountryForReturnDto> actual_CountryForReturnDto_List = await _countriesService.GetAllCoutries();

            actual_CountryForReturnDto_List.Should().BeEquivalentTo(expected_countryForReturnDTO_List);
        }

        #endregion GetAllCountries

        #region GetCountryById

        [Fact]
        public async Task GetCountryByID_NullId()
        {
            //Arrange
            Guid? Id = null;

            //Act
            CountryForReturnDto? countryForReturnDTO = await _countriesService.GetCountryById(Id);

            //Assert
            Assert.Null(countryForReturnDTO);
        }

        [Fact]
        public async Task GetCountryByID_ValidId()
        {
            //Arrange
            Country Country = _Fixture.Create<Country>();

            var expectd = Country.ToCountryForReturn();

            _CountriesRepositoryMock.Setup(c => c.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(Country);

            //Act
            CountryForReturnDto? actual = await _countriesService.GetCountryById(Country.Id);

            actual.Should().Be(expectd);
        }

        #endregion GetCountryById
    }
}

/*
 Isolate Tests
Downlaod Moq
Downlaod EFCoremock.moq
set all Dbset option to fake and ovrride it
 */