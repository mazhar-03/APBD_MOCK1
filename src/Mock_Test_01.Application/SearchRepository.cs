using Microsoft.Data.SqlClient;
using Mock_Test_01.Models;

namespace Mock_Test_01.Application;

public class SearchRepository : ISearchRepository
{
    private readonly string _connectionString;

    public SearchRepository(string connectionString)
    {
        _connectionString = connectionString;
    }


    public CountryDto GetCurrenciesForCountry(string countryName)
    {
        var result = new CountryDto
        {
            CountryName = countryName,
            Currencies = new List<CurrencyDto>()
        };

        var sql = @"SELECT cu.Name, cu.Rate FROM Currency cu
                    JOIN Currency_Country cc ON cu.Id = cc.Currency_Id
                    JOIN Country co ON co.Id = cc.Country_Id
                    WHERE co.Name = @countryName";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@countryName", countryName);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
                while (reader.Read())
                {
                    var currencyDto = new CurrencyDto
                    {
                        CurrencyName = reader.GetString(0),
                        Rate = reader.GetFloat(1)
                    };
                    ((List<CurrencyDto>)result.Currencies).Add(currencyDto);
                }
        }

        return result;
    }

    public CurrencyCountriesDto GetCountriesForCurrency(string currencyName)
    {
        var result = new CurrencyCountriesDto
        {
            CurrencyName = currencyName,
            Countries = new List<CountryInfoDto>()
        };

        var sql = @"SELECT co.Name as CountryName FROM Currency cu
                    JOIN Currency_Country cc ON cu.Id = cc.Currency_Id
                    JOIN Country co ON co.Id = cc.Country_Id
                    WHERE cu.Name = @countryName";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@countryName", currencyName);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
                while (reader.Read())
                {
                    var countryInfoDto = new CountryInfoDto
                    {
                        CountryName = reader.GetString(0)
                    };
                    ((List<CountryInfoDto>)result.Countries).Add(countryInfoDto);
                }
        }

        return result;
    }
}