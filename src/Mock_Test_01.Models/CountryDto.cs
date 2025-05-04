namespace Mock_Test_01.Models;

public class CountryDto
{
    public string CountryName { get; set; }
    public IEnumerable<CurrencyDto> Currencies { get; set; }
}

public class CurrencyDto
{
    public string CurrencyName { get; set; }
    public float Rate { get; set; }
}