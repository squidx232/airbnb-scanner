using AirbnbKeywordFinder.Core.Models;

namespace AirbnbKeywordFinder.Core.Services;

public interface ISearchApiClient
{
    Task<AirbnbSearchResponse> SearchAsync(AirbnbSearchRequest request, string? customApiKey = null, CancellationToken ct = default);
    Task<AirbnbPropertyResponse> GetPropertyDetailsAsync(string propertyId, DateOnly? checkIn = null, DateOnly? checkOut = null, string? customApiKey = null, CancellationToken ct = default);
}
