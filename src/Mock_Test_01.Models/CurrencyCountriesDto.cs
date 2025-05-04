namespace Mock_Test_01.Models;

public class CurrencyCountriesDto
{
    public string CurrencyName { get; set; }
    public IEnumerable<CountryInfoDto> Countries { get; set; }
}

public class CountryInfoDto
{
    public string CountryName { get; set; }
}