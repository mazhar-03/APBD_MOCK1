namespace Mock_Test_01.Models;

public class CurrencyDto
{
    public string CurrencyName { get; set; }
    public double Rate { get; set; }
}
public class CountryDto
{
    public string CountryName { get; set; }
    public List<CurrencyDto> Currencies { get; set; } = new List<CurrencyDto>();  // Initialize here
}


