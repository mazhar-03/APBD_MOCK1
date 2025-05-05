using Mock_Test_01.Models;

namespace Mock_Test_01.Application;

public interface ISearchRepository
{
    CountryDto GetCurrenciesForCountry(string countryName);
    CurrencyCountriesDto GetCountriesForCurrency(string currencyName);
    Currency GetCurrencyByName(string currencyName);
    void CreateOrUpdateCurrency(string currencyName, float rate, List<string> countryNames);
    void UpdateCurrency(int currencyId, float rate);
    bool CurrencyExists(string currencyName);
    void LinkCurrencyWithCountries(int currencyId, List<string> countryNames);
    int InsertCurrency(string currencyName, float rate);
}