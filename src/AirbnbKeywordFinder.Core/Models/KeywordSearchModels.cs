namespace AirbnbKeywordFinder.Core.Models;

public class KeywordSearchOptions
{
    public List<string> Keywords { get; set; } = new();
    public bool MatchAll { get; set; } = false;
    public bool CaseSensitive { get; set; } = false;
    public bool WholeWordOnly { get; set; } = false;
    public int MaxPages { get; set; } = 2;
    public int MaxResults { get; set; } = 10;
}

public class KeywordSearchResult
{
    public List<MatchedProperty> MatchedProperties { get; set; } = new();
    public int TotalPropertiesScanned { get; set; }
    public int PagesSearched { get; set; }
    public string? SearchLocation { get; set; }
    public List<string> Keywords { get; set; } = new();
    public bool MatchAllKeywords { get; set; }
    public TimeSpan Duration { get; set; }
    public int ApiCreditsUsed { get; set; }
}

public class MatchedProperty
{
    public AirbnbProperty Property { get; set; } = null!;
    public AirbnbPropertyDetails? FullDetails { get; set; }
    public HostInfo? Host { get; set; }
    public List<string> MatchedKeywords { get; set; } = new();
    public List<string> MatchContexts { get; set; } = new();
}

public class SearchProgress
{
    public string Message { get; set; } = "";
    public int PropertiesScanned { get; set; }
    public int MatchesFound { get; set; }
    public int CurrentPage { get; set; }
    public bool IsComplete { get; set; }
    public int CreditsUsed { get; set; }
    
    /// <summary>
    /// If a match was found, this contains the matched property.
    /// </summary>
    public MatchedProperty? MatchedProperty { get; set; }
}
