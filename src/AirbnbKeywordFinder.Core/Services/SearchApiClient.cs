using System.Text.Json;
using System.Web;
using AirbnbKeywordFinder.Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AirbnbKeywordFinder.Core.Services;

public class SearchApiClient : ISearchApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private const string BaseUrl = "https://www.searchapi.io/api/v1/search";
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SearchApiClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<AirbnbSearchResponse> SearchAsync(AirbnbSearchRequest request, string? customApiKey = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customApiKey))
            throw new InvalidOperationException("API key is required. Please provide your SearchAPI key.");

        var queryParams = request.ToQueryParameters();
        queryParams["api_key"] = customApiKey;

        var cacheKey = $"search_{string.Join("_", queryParams.Where(k => k.Key != "api_key").OrderBy(k => k.Key).Select(k => $"{k.Key}={k.Value}"))}";
        
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
        if (string.IsNullOrWhiteSpace(customApiKey))
            throw new InvalidOperationException("API key is required. Please provide your SearchAPI key.");

        var cacheKey = $"property_{propertyId}_{checkIn}_{checkOut}";
        
        if (_cache.TryGetValue(cacheKey, out AirbnbPropertyResponse? cached) && cached != null)
            return cached;

        var queryParams = new Dictionary<string, string>
        {
            ["engine"] = "airbnb_property",
            ["property_id"] = propertyId,
            ["api_key"] = customApiKey
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

    public async Task<HostInfo?> GetHostInfoAsync(string hostUserId, string? customApiKey = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(customApiKey)) return null;

        var cacheKey = $"host_info_{hostUserId}";
        if (_cache.TryGetValue(cacheKey, out HostInfo? cachedHost) && cachedHost != null)
            return cachedHost;

        try
        {
            var queryParams = new Dictionary<string, string>
            {
                ["engine"] = "airbnb",
                ["airbnb_user_id"] = hostUserId,
                ["api_key"] = customApiKey
            };

            var url = BuildUrl(queryParams);
            var response = await _httpClient.GetAsync(url, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode) return null;

            var result = JsonSerializer.Deserialize<AirbnbSearchResponse>(content, _jsonOptions);
            if (result?.Properties == null || result.Properties.Count == 0) return null;

            var firstPropertyId = result.Properties[0].Id;
            if (string.IsNullOrEmpty(firstPropertyId)) return null;

            var details = await GetPropertyDetailsAsync(firstPropertyId, null, null, customApiKey, ct);
            if (details?.Host == null) return null;

            _cache.Set(cacheKey, details.Host, TimeSpan.FromMinutes(5));
            return details.Host;
        }
        catch
        {
            return null;
        }
    }

    private static string BuildUrl(Dictionary<string, string> queryParams)
    {
        var queryString = string.Join("&", queryParams.Select(kvp => 
            $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
        return $"{BaseUrl}?{queryString}";
    }
}
