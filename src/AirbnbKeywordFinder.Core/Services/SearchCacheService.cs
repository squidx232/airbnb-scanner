using Microsoft.Extensions.Caching.Memory;

namespace AirbnbKeywordFinder.Core.Services;

/// <summary>
/// Caches searched property IDs to avoid duplicate API calls across sessions.
/// </summary>
public interface ISearchCacheService
{
    bool HasBeenSearched(string propertyId);
    void MarkAsSearched(string propertyId);
    int GetSearchedCount();
    void Clear();
}

public class SearchCacheService : ISearchCacheService
{
    private readonly HashSet<string> _searchedPropertyIds = new();
    private readonly object _lock = new();

    public bool HasBeenSearched(string propertyId)
    {
        lock (_lock)
        {
            return _searchedPropertyIds.Contains(propertyId);
        }
    }

    public void MarkAsSearched(string propertyId)
    {
        lock (_lock)
        {
            _searchedPropertyIds.Add(propertyId);
        }
    }

    public int GetSearchedCount() 
    {
        lock (_lock)
        {
            return _searchedPropertyIds.Count;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _searchedPropertyIds.Clear();
        }
    }
}
