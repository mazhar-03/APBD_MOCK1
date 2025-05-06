using Mock_Test_01.Models;

namespace Mock_Test_01.Application;

public interface ISearchRepository
{
    CountryDto GetCurrenciesForCountry(string countryName);
    CurrencyCountriesDto GetCountriesForCurrency(string currencyName);
    Currency GetCurrencyByName(string currencyName);
    void CreateOrUpdateCurrency(string currencyName, double rate, List<string> countryNames);
    void UpdateCurrency(int currencyId, double rate);
    bool CurrencyExists(string currencyName);
    void LinkCurrencyWithCountries(int currencyId, List<string> countryNames);
    int InsertCurrency(string currencyName, double rate);
    int GetCountryIdByName(string countryName);
}