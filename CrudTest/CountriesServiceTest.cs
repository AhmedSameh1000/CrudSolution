using ServiceContract.DTOs;
using ServiceContract.Interfaces;
using Services;

namespace CrudTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region AddCountry

        //When CountryForCreateDto is null, it should throw ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryForCreateDto? countryForCreateDTO = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _countriesService.AddCountry(countryForCreateDTO);
            });
        }

        //When the Name is null, it should throw ArgumentException
        [Fact]
        public void AddCountry_NameIsNull()
        {
            //Arrange
            CountryForCreateDto? countryForCreateDTO = new CountryForCreateDto() { Name = null };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(countryForCreateDTO);
            });
        }

        //When the Name is duplicate, it should throw ArgumentException
        [Fact]
        public void AddCountry_DuplicateName()
        {
            //Arrange
            CountryForCreateDto? countryForCreateDTO1 = new CountryForCreateDto() { Name = "Egypt" };
            CountryForCreateDto? countryForCreateDTO2 = new CountryForCreateDto() { Name = "Egypt" };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(countryForCreateDTO1);
                _countriesService.AddCountry(countryForCreateDTO2);
            });
        }

        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryForCreateDto? countryForCreateDTO = new CountryForCreateDto() { Name = "Palestine" };

            //Act
            CountryForReturnDto countryForReturnDTO = _countriesService.AddCountry(countryForCreateDTO);
            List<CountryForReturnDto> actual_CountryForReturnDto_List = _countriesService.GetAllCoutries();

            //Assert
            Assert.True(countryForReturnDTO.Id != Guid.Empty);
            Assert.Contains(countryForReturnDTO, actual_CountryForReturnDto_List);
            //actual_CountryForReturnDto_List.ForEach(ac => Assert.Equivalent(ac, countryForReturnDTO));
        }

        #endregion AddCountry

        #region GetAllCountries

        [Fact]
        public void GetAllCountries_EmptyList()
        {
            //Act
            List<CountryForReturnDto> actualCountryForReturnList = _countriesService.GetAllCoutries();

            //Assert
            Assert.Empty(actualCountryForReturnList);
        }

        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryForCreateDto> countryForCreateList = new() {
                    new(){ Name = "Iraq" },
                    new(){ Name = "Syria" }};
            //Act  expected CountryForReturnDto List
            List<CountryForReturnDto> expected_countryForReturnDTO_List = new();

            countryForCreateList.ForEach(cf => expected_countryForReturnDTO_List.Add(_countriesService.AddCountry(cf)));

            List<CountryForReturnDto> actual_CountryForReturnDto_List = _countriesService.GetAllCoutries();

            //Check that actual_CountryForReturnDto_List contains All expected_countryForReturn_List items
            expected_countryForReturnDTO_List.ForEach(ec => Assert.Contains(ec, actual_CountryForReturnDto_List));
        }

        #endregion GetAllCountries

        #region GetCountryById

        [Fact]
        public void GetCountryByID_NullId()
        {
            //Arrange
            Guid? Id = null;

            //Act
            CountryForReturnDto? countryForReturnDTO = _countriesService.GetCountryById(Id);

            //Assert
            Assert.Null(countryForReturnDTO);
        }

        [Fact]
        public void GetCountryByID_ValidId()
        {
            //Arrange
            CountryForCreateDto? countryForCreateDTO = new() { Name = "Libya" };
            CountryForReturnDto countryForReturnDTO_From_Add = _countriesService.AddCountry(countryForCreateDTO);

            //Act
            CountryForReturnDto? countryForReturnDTO_From_Get = _countriesService.GetCountryById(countryForReturnDTO_From_Add.Id);

            //Assert
            Assert.Equal(countryForReturnDTO_From_Add, countryForReturnDTO_From_Get);
        }

        #endregion GetCountryById
    }
}