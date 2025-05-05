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

    public Currency GetCurrencyByName(string currencyName)
    {
        var sql = "SELECT * FROM Currency WHERE Name = @currencyName";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@currencyName", currencyName);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
                while (reader.Read())
                    return new Currency
                    (
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetDouble(2)
                    );
        }

        return null;
    }

    public bool CurrencyExists(string currencyName)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT COUNT(1) FROM Currency WHERE Name = @CurrencyName";

            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@CurrencyName", currencyName);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }


    public void UpdateCurrency(int currencyId, float rate)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = "UPDATE Currency SET Rate = @Rate WHERE Id = @CurrencyId";
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Rate", rate);
                cmd.Parameters.AddWithValue("@CurrencyId", currencyId);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void LinkCurrencyWithCountries(int currencyId, List<string> countryNames)
    {
        foreach (var countryName in countryNames)
        {
            var countryId = GetCountryIdByName(countryName);
            if (countryId == -1) throw new ArgumentException($"Country '{countryName}' does not exist.");

            InsertCurrencyCountryLink(currencyId, countryId);
        }
    }

    public void CreateOrUpdateCurrency(string currencyName, float rate, List<string> countryNames)
    {
        var existingCurrency = GetCurrencyByName(currencyName);
        if (existingCurrency != null)
        {
        }
    }

    public int InsertCurrency(string currencyName, float rate)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = "INSERT INTO Currency (Name, Rate) VALUES (@CurrencyName, @Rate); SELECT SCOPE_IDENTITY();";

            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@CurrencyName", currencyName);
                cmd.Parameters.AddWithValue("@Rate", rate);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }

    public int GetCountryIdByName(string countryName)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT Id FROM Country WHERE Name = @CountryName";

            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@CountryName", countryName);
                var result = cmd.ExecuteScalar();

                if (result != null) return Convert.ToInt32(result);

                return -1;
            }
        }
    }

    public void InsertCurrencyCountryLink(int currencyId, int countryId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var query = @"
            INSERT INTO Currency_Country (Currency_Id, Country_Id)
            VALUES (@CurrencyId, @CountryId)";

            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@CurrencyId", currencyId);
                cmd.Parameters.AddWithValue("@CountryId", countryId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}