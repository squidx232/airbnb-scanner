using Microsoft.AspNetCore.Mvc;
using AirbnbKeywordFinder.Core.Services;
using AirbnbKeywordFinder.Core.Models;

namespace AirbnbKeywordFinder.Web.Controllers;

[ApiController]
[Route("api")]
public class ApiProxyController : ControllerBase
{
    private readonly ISearchApiClient _apiClient;

    public ApiProxyController(ISearchApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] AirbnbSearchRequest request, [FromHeader(Name = "X-Api-Key")] string? apiKey)
    {
        try
        {
            var result = await _apiClient.SearchAsync(request, apiKey);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetProperty(
        string propertyId, 
        [FromQuery] string? checkIn, 
        [FromQuery] string? checkOut,
        [FromHeader(Name = "X-Api-Key")] string? apiKey)
    {
        try
        {
            DateOnly? checkInDate = string.IsNullOrEmpty(checkIn) ? null : DateOnly.Parse(checkIn);
            DateOnly? checkOutDate = string.IsNullOrEmpty(checkOut) ? null : DateOnly.Parse(checkOut);
            
            var result = await _apiClient.GetPropertyDetailsAsync(propertyId, checkInDate, checkOutDate, apiKey);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
