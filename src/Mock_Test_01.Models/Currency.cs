namespace Mock_Test_01.Models;

public class Currency
{
    public Currency(int id, string name, double rate)
    {
        Id = id;
        Name = name;
        Rate = rate;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public double Rate { get; set; }
}
