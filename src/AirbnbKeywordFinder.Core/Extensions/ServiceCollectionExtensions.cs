using AirbnbKeywordFinder.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AirbnbKeywordFinder.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAirbnbKeywordFinder(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpClient<ISearchApiClient, SearchApiClient>();
        services.AddSingleton<ISearchCacheService, SearchCacheService>(); // Singleton to persist across searches
        services.AddScoped<IKeywordSearchService, KeywordSearchService>();
        return services;
    }
}
