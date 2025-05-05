namespace Mock_Test_01.Models;

public class CurrencyRequestDto
{
    public string CurrencyName { get; set; }
    public float Rate { get; set; }
    public List<string> Countries { get; set; }
}