using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AirbnbKeywordFinder.Core.Models;

namespace AirbnbKeywordFinder.Core.Services;

public class KeywordSearchService : IKeywordSearchService
{
    private readonly ISearchApiClient _apiClient;
    private readonly ISearchCacheService _cacheService;

    public KeywordSearchService(ISearchApiClient apiClient, ISearchCacheService cacheService)
    {
        _apiClient = apiClient;
        _cacheService = cacheService;
    }

    public async IAsyncEnumerable<SearchProgress> SearchWithProgressAsync(
        AirbnbSearchRequest searchRequest,
        KeywordSearchOptions keywordOptions,
        string? customApiKey = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var result = new KeywordSearchResult
        {
            SearchLocation = searchRequest.Query,
            Keywords = keywordOptions.Keywords.ToList(),
            MatchAllKeywords = keywordOptions.MatchAll
        };

        var stopwatch = Stopwatch.StartNew();
        var creditsUsed = 0;
        string? pageToken = null;
        var skippedCount = 0;

        yield return new SearchProgress { Message = $"Searching in {searchRequest.Query}...", CreditsUsed = creditsUsed };

        for (int page = 0; page < keywordOptions.MaxPages && !ct.IsCancellationRequested; page++)
        {
            searchRequest.PageToken = pageToken;

            yield return new SearchProgress
            {
                Message = $"Fetching page {page + 1}...",
                CurrentPage = page + 1,
                PropertiesScanned = result.TotalPropertiesScanned,
                MatchesFound = result.MatchedProperties.Count,
                CreditsUsed = creditsUsed
            };

            AirbnbSearchResponse? searchResponse = null;
            string? searchError = null;

            try
            {
                searchResponse = await _apiClient.SearchAsync(searchRequest, customApiKey, ct);
                creditsUsed++;
            }
            catch (Exception ex)
            {
                searchError = ex.Message;
            }

            if (searchError != null)
            {
                yield return new SearchProgress { Message = $"Search failed: {searchError}", IsComplete = true, CreditsUsed = creditsUsed };
                yield break;
            }

            if (searchResponse == null || searchResponse.Properties.Count == 0)
            {
                yield return new SearchProgress { Message = "No properties found", IsComplete = true, CreditsUsed = creditsUsed };
                yield break;
            }

            var pageSkippedCount = 0;
            var pageProcessedCount = 0;

            foreach (var property in searchResponse.Properties)
            {
                if (ct.IsCancellationRequested || result.MatchedProperties.Count >= keywordOptions.MaxResults)
                    break;

                if (string.IsNullOrEmpty(property.Id)) continue;

                // Skip if already searched in a previous run
                if (_cacheService.HasBeenSearched(property.Id))
                {
                    pageSkippedCount++;
                    skippedCount++;
                    yield return new SearchProgress
                    {
                        Message = $"Skipped (cached): {Truncate(property.Title, 40)}",
                        PropertiesScanned = result.TotalPropertiesScanned,
                        MatchesFound = result.MatchedProperties.Count,
                        CurrentPage = page + 1,
                        CreditsUsed = creditsUsed
                    };
                    continue;
                }

                pageProcessedCount++;
                result.TotalPropertiesScanned++;

                yield return new SearchProgress
                {
                    Message = $"Checking: {Truncate(property.Title, 40)}",
                    PropertiesScanned = result.TotalPropertiesScanned,
                    MatchesFound = result.MatchedProperties.Count,
                    CurrentPage = page + 1,
                    CreditsUsed = creditsUsed
                };

                AirbnbPropertyResponse? details = null;
                string? detailsError = null;

                try
                {
                    details = await _apiClient.GetPropertyDetailsAsync(
                        property.Id, searchRequest.CheckInDate, searchRequest.CheckOutDate, customApiKey, ct);
                    creditsUsed++;
                    _cacheService.MarkAsSearched(property.Id);
                }
                catch (Exception ex)
                {
                    detailsError = ex.Message;
                }

                if (detailsError != null)
                {
                    yield return new SearchProgress
                    {
                        Message = $"Failed to fetch details: {detailsError}",
                        PropertiesScanned = result.TotalPropertiesScanned,
                        MatchesFound = result.MatchedProperties.Count,
                        CreditsUsed = creditsUsed
                    };
                    continue;
                }

                if (details?.Property == null) continue;

                var matched = MatchProperty(details.Property, property, details.Host, keywordOptions);
                if (matched != null)
                {
                    result.MatchedProperties.Add(matched);
                    yield return new SearchProgress
                    {
                        Message = $"MATCH: {Truncate(property.Title, 40)} [{string.Join(", ", matched.MatchedKeywords)}]",
                        PropertiesScanned = result.TotalPropertiesScanned,
                        MatchesFound = result.MatchedProperties.Count,
                        CurrentPage = page + 1,
                        CreditsUsed = creditsUsed,
                        MatchedProperty = matched
                    };
                }

                await Task.Delay(50, ct);
            }

            // If all items on this page were cached, notify and continue to next page
            if (pageSkippedCount > 0 && pageProcessedCount == 0)
            {
                yield return new SearchProgress
                {
                    Message = $"Page {page + 1}: All {pageSkippedCount} listings were cached, fetching next page...",
                    PropertiesScanned = result.TotalPropertiesScanned,
                    MatchesFound = result.MatchedProperties.Count,
                    CurrentPage = page + 1,
                    CreditsUsed = creditsUsed
                };
            }

            if (result.MatchedProperties.Count >= keywordOptions.MaxResults)
                break;

            pageToken = searchResponse.Pagination?.NextPageToken;
            if (string.IsNullOrEmpty(pageToken)) 
            {
                // No more pages available
                if (skippedCount > 0 && result.TotalPropertiesScanned == 0)
                {
                    yield return new SearchProgress
                    {
                        Message = $"All {skippedCount} listings were previously cached. Use 'Clear Cache' to re-scan.",
                        PropertiesScanned = result.TotalPropertiesScanned,
                        MatchesFound = result.MatchedProperties.Count,
                        IsComplete = true,
                        CreditsUsed = creditsUsed
                    };
                    yield break;
                }
                break;
            }
        }

        stopwatch.Stop();
        result.Duration = stopwatch.Elapsed;
        result.PagesSearched = Math.Min(keywordOptions.MaxPages, result.TotalPropertiesScanned > 0 ? 1 : 0);
        result.ApiCreditsUsed = creditsUsed;

        var summaryMsg = result.TotalPropertiesScanned > 0
            ? $"Complete! Found {result.MatchedProperties.Count} matches from {result.TotalPropertiesScanned} new properties"
            : $"Complete! All listings were cached ({skippedCount} skipped). Clear cache to re-scan.";

        yield return new SearchProgress
        {
            Message = summaryMsg,
            PropertiesScanned = result.TotalPropertiesScanned,
            MatchesFound = result.MatchedProperties.Count,
            IsComplete = true,
            CreditsUsed = creditsUsed
        };
    }

    private MatchedProperty? MatchProperty(AirbnbPropertyDetails details, AirbnbProperty basic, HostInfo? host, KeywordSearchOptions options)
    {
        if (options.Keywords.Count == 0) return null;

        var searchText = details.GetSearchableText();
        if (host != null)
            searchText += $" {host.Name} {host.About}";

        var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        var matchedKeywords = new List<string>();
        var contexts = new List<string>();

        foreach (var keyword in options.Keywords)
        {
            bool isMatch = options.WholeWordOnly
                ? Regex.IsMatch(searchText, $@"\b{Regex.Escape(keyword)}\b",
                    options.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase)
                : searchText.Contains(keyword, comparison);

            if (isMatch)
            {
                matchedKeywords.Add(keyword);
                contexts.Add(ExtractContext(searchText, keyword, comparison));
            }
        }

        bool hasMatch = options.MatchAll
            ? matchedKeywords.Count == options.Keywords.Count
            : matchedKeywords.Count > 0;

        if (!hasMatch) return null;

        return new MatchedProperty
        {
            Property = basic,
            FullDetails = details,
            Host = host,
            MatchedKeywords = matchedKeywords,
            MatchContexts = contexts
        };
    }

    private static string ExtractContext(string text, string keyword, StringComparison comparison)
    {
        var idx = text.IndexOf(keyword, comparison);
        if (idx < 0) return "";

        var start = Math.Max(0, idx - 40);
        var end = Math.Min(text.Length, idx + keyword.Length + 40);
        var prefix = start > 0 ? "..." : "";
        var suffix = end < text.Length ? "..." : "";

        return $"{prefix}{text[start..end]}{suffix}";
    }

    private static string Truncate(string? text, int maxLength) =>
        string.IsNullOrEmpty(text) ? "" : text.Length <= maxLength ? text : text[..(maxLength - 3)] + "...";
}
