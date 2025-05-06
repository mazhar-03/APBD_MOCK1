using Microsoft.Data.SqlClient;
using Mock_Test_01.Models;

namespace Mock_Test_01.Application
{
    public class SearchRepository : ISearchRepository
    {
        private readonly string _connectionString;

        public SearchRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Get list of currencies for a given country
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
                {
                    while (reader.Read())
                    {
                        var currencyDto = new CurrencyDto
                        {
                            CurrencyName = reader.GetString(0),
                            Rate = Convert.ToDouble(reader.GetFloat(1))
                        };
                        result.Currencies.Add(currencyDto);
                    }
                }
            }

            return result;
        }

        // Check if currency already exists in the database
        public bool CurrencyExists(string currencyName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(1) FROM Currency WHERE Name = @CurrencyName";

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@CurrencyName", currencyName);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        // Get list of countries for a given currency
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
                        WHERE cu.Name = @currencyName";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@currencyName", currencyName);

                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var countryInfoDto = new CountryInfoDto
                        {
                            CountryName = reader.GetString(0)
                        };
                        result.Countries.Add(countryInfoDto);
                    }
                }
            }

            return result;
        }

        // Get a currency by its name
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
                {
                    while (reader.Read())
                    {
                        return new Currency
                        (
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetDouble(2)
                        );
                    }
                }
            }

            return null;
        }

        // Create or update a currency
        public void CreateOrUpdateCurrency(string currencyName, double rate, List<string> countryNames)
        {
            var existingCurrency = GetCurrencyByName(currencyName);
            if (existingCurrency != null)
            {
                // Update existing currency
                UpdateCurrency(existingCurrency.Id, rate);
                LinkCurrencyWithCountries(existingCurrency.Id, countryNames);
            }
            else
            {
                // Insert new currency
                var currencyId = InsertCurrency(currencyName, rate);
                LinkCurrencyWithCountries(currencyId, countryNames);
            }
        }


        // Link currency with multiple countries
        public void LinkCurrencyWithCountries(int currencyId, List<string> countryNames)
        {
            foreach (var countryName in countryNames)
            {
                var countryId = GetCountryIdByName(countryName);
                if (countryId == -1)
                {
                    throw new ArgumentException($"Country '{countryName}' does not exist.");
                }

                // Insert the relationship between currency and country
                InsertCurrencyCountryLink(currencyId, countryId);
            }
        }

        // Insert a new currency into the database
        public int InsertCurrency(string currencyName, double rate)
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

        // Update the rate of an existing currency
        public void UpdateCurrency(int currencyId, double rate)
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

        // Insert a link between a currency and a country
        public void InsertCurrencyCountryLink(int currencyId, int countryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Currency_Country (Currency_Id, Country_Id) VALUES (@CurrencyId, @CountryId)";

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@CurrencyId", currencyId);
                    cmd.Parameters.AddWithValue("@CountryId", countryId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Get the ID of a country by its name
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

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }

                    return -1;
                }
            }
        }
    }
}
