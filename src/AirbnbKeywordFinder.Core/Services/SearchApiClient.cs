using System.Text.Json;
using System.Web;
using AirbnbKeywordFinder.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace AirbnbKeywordFinder.Core.Services;

public class SearchApiClient : ISearchApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SearchApiClient(HttpClient httpClient, IMemoryCache cache, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _cache = cache;
        _apiKey = configuration["SearchApi:ApiKey"] ?? throw new InvalidOperationException("SearchApi:ApiKey is required");
        _baseUrl = configuration["SearchApi:BaseUrl"] ?? "https://www.searchapi.io/api/v1/search";
    }

    public async Task<AirbnbSearchResponse> SearchAsync(AirbnbSearchRequest request, string? customApiKey = null, CancellationToken ct = default)
    {
        var queryParams = request.ToQueryParameters();
        queryParams["api_key"] = !string.IsNullOrWhiteSpace(customApiKey) ? customApiKey : _apiKey;

        var cacheKey = $"search_{string.Join("_", queryParams.OrderBy(k => k.Key).Select(k => $"{k.Key}={k.Value}"))}";
        
        if (_cache.TryGetValue(cacheKey, out AirbnbSearchResponse? cached) && cached != null)
            return cached;

        var url = BuildUrl(queryParams);
        var response = await _httpClient.GetAsync(url, ct);
        var content = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {content}");

        var result = JsonSerializer.Deserialize<AirbnbSearchResponse>(content, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");

        if (!string.IsNullOrEmpty(result.Error))
            throw new InvalidOperationException($"API error: {result.Error}");

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
        return result;
    }

    public async Task<AirbnbPropertyResponse> GetPropertyDetailsAsync(string propertyId, DateOnly? checkIn = null, DateOnly? checkOut = null, string? customApiKey = null, CancellationToken ct = default)
    {
        var cacheKey = $"property_{propertyId}_{checkIn}_{checkOut}";
        
        if (_cache.TryGetValue(cacheKey, out AirbnbPropertyResponse? cached) && cached != null)
            return cached;

        var apiKeyToUse = !string.IsNullOrWhiteSpace(customApiKey) ? customApiKey : _apiKey;
        var queryParams = new Dictionary<string, string>
        {
            ["engine"] = "airbnb_property",
            ["property_id"] = propertyId,
            ["api_key"] = apiKeyToUse
        };

        if (checkIn.HasValue) queryParams["check_in_date"] = checkIn.Value.ToString("yyyy-MM-dd");
        if (checkOut.HasValue) queryParams["check_out_date"] = checkOut.Value.ToString("yyyy-MM-dd");

        var url = BuildUrl(queryParams);
        var response = await _httpClient.GetAsync(url, ct);
        var content = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {content}");

        var result = JsonSerializer.Deserialize<AirbnbPropertyResponse>(content, _jsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");

        if (!string.IsNullOrEmpty(result.Error))
            throw new InvalidOperationException($"API error: {result.Error}");

        _cache.Set(cacheKey, result, TimeSpan.FromHours(1));
        return result;
    }

    private string BuildUrl(Dictionary<string, string> queryParams)
    {
        var queryString = string.Join("&", queryParams.Select(kvp => 
            $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
        return $"{_baseUrl}?{queryString}";
    }
}
