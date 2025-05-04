using Mock_Test_01.Models;

namespace Mock_Test_01.Application;

public interface ISearchRepository
{
    CountryDto GetCurrenciesForCountry(string countryName);
    CurrencyCountriesDto GetCountriesForCurrency(string currencyName);
}