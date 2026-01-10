using AirbnbKeywordFinder.Core.Models;

namespace AirbnbKeywordFinder.Core.Services;

public interface IKeywordSearchService
{
    IAsyncEnumerable<SearchProgress> SearchWithProgressAsync(
        AirbnbSearchRequest searchRequest,
        KeywordSearchOptions keywordOptions,
        string? customApiKey = null,
        CancellationToken ct = default);
}
