using Microsoft.AspNetCore.Mvc;
using Mock_Test_01.Application;
using Mock_Test_01.Models;

namespace Mock_Test_01.API.controller;

[Route("api/")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISearchRepository _searchRepository;

    public SearchController(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    [HttpGet]
    [Route("search/")]
    public IResult GetByCountryName([FromQuery] string type, [FromQuery] string query)
    {
        if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(query))
            return Results.BadRequest("Both search type and query parameters are required.");

        if (type.ToLower() == "country")
        {
            var result = _searchRepository.GetCurrenciesForCountry(query);

            if (!result.Currencies.Any())
                return Results.NotFound("Country not found or no currencies available.");

            return Results.Ok(new
            {
                CountryName = query,
                result.Currencies
            });
        }

        if (type.ToLower() == "currency")
        {
            var result = _searchRepository.GetCountriesForCurrency(query);

            if (!result.Countries.Any())
                return Results.NotFound("Currency not found or no countries using it.");

            var countryNames = result.Countries.Select(c => c.CountryName).ToList();
            return Results.Ok(countryNames);
        }

        return Results.BadRequest("Invalid 'type' parameter. Use 'country' or 'currency'.");
    }

    [HttpPost]
    [Route("currency/")]
    public IResult CreateOrUpdateCurrency([FromBody] CurrencyRequestDto request)
    {
        if (string.IsNullOrEmpty(request.CurrencyName) || request.Rate <= 0 || !request.Countries.Any())
            return Results.BadRequest("Invalid request data.");
        try
        {
            foreach (var countryName in request.Countries)
            {
                var countryId = _searchRepository.GetCountryIdByName(countryName);
                if (countryId == -1)
                    return Results.BadRequest($"Country {countryName} not found.");
            }

            _searchRepository.CreateOrUpdateCurrency(request.CurrencyName, request.Rate, request.Countries);
            return Results.Ok("Currency successfully created/updated.");
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"An error occured while upserting currency: {ex.Message}");
        }
    }
}