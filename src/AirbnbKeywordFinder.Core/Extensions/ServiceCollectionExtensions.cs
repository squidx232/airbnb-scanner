using AirbnbKeywordFinder.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AirbnbKeywordFinder.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAirbnbKeywordFinder(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpClient<ISearchApiClient, SearchApiClient>();
        services.AddSingleton<ISearchCacheService, SearchCacheService>();
        services.AddScoped<IKeywordSearchService, KeywordSearchService>();
        return services;
    }

    /// <summary>
    /// Registers services for Blazor WebAssembly (client-side).
    /// </summary>
    public static IServiceCollection AddAirbnbKeywordFinderWasm(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped(sp => new HttpClient());
        services.AddScoped<ISearchApiClient, SearchApiClient>();
        services.AddSingleton<ISearchCacheService, SearchCacheService>();
        services.AddScoped<IKeywordSearchService, KeywordSearchService>();
        return services;
    }
}
