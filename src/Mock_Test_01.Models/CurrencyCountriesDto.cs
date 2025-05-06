namespace Mock_Test_01.Models;

public class CurrencyCountriesDto
{
    public string CurrencyName { get; set; }
    public List<CountryInfoDto> Countries { get; set; } = new List<CountryInfoDto>(); 
}
public class CountryInfoDto
{
    public string CountryName { get; set; }
}