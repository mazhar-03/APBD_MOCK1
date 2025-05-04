using Microsoft.AspNetCore.Mvc;
using Mock_Test_01.Application;

namespace Mock_Test_01.API.controller;

[Route("api/search")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISearchRepository _searchRepository;

    public SearchController(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    [HttpGet]
    public IResult GetByCountryName([FromQuery] string type, [FromQuery] string query)
    {
        if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(query))
            return Results.BadRequest("Both search type and query parameters are required.");

        if (type.ToLower() == "country")
        {
            var result = _searchRepository.GetCurrenciesForCountry(query);
            
            if(result== null || !result.Currencies.Any())
                return Results.NotFound("Country not found or no currencies available.");
            
            return Results.Ok(new
            {
                CountryName = type,
                Currencies = result.Currencies
            });
        }
        else if (type.ToLower() == "currency")
        {
            var result = _searchRepository.GetCountriesForCurrency(query);
            
            if (result == null || !result.Countries.Any())
            {
                return Results.NotFound("Currency not found or no countries using it.");
            }

            return Results.Ok(new
            {
                CurrencyName = query,
                Countries = result.Countries
            });
        }
        else
        {
            return Results.BadRequest("Invalid 'type' parameter. Use 'country' or 'currency'.");
        }
    }
}
