using AirbnbKeywordFinder.Core.Models;

namespace AirbnbKeywordFinder.Core.Services;

/// <summary>
/// Provides demo/mock data for UI testing without using API credits.
/// </summary>
public static class DemoDataService
{
    public static List<MatchedProperty> GetDemoResults(string location, List<string> keywords)
    {
        return new List<MatchedProperty>
        {
            new()
            {
                Property = new AirbnbProperty
                {
                    Id = "demo1",
                    Title = "Stunning Oceanfront Villa with Private Pool",
                    Description = $"Luxury villa in {location}",
                    Rating = 4.95,
                    Reviews = 128,
                    Price = new PriceInfo { TotalPrice = "$1,250", ExtractedTotalPrice = 1250, Qualifier = "for 5 nights", ExtractedPricePerQualifier = 250 },
                    Accommodations = new List<string> { "4 bedrooms", "3 bathrooms", "8 guests" },
                    Badges = new List<string> { "Superhost", "Guest favorite" },
                    Images = new List<string> { "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800" },
                    Link = "https://airbnb.com/rooms/demo1"
                },
                FullDetails = new AirbnbPropertyDetails
                {
                    Title = "Stunning Oceanfront Villa with Private Pool",
                    Description = $"Welcome to our breathtaking oceanfront villa! This stunning property offers panoramic ocean views, a private infinity pool, and direct beach access. Perfect for families or groups looking for a luxurious getaway in {location}. The villa features modern amenities, a fully equipped gourmet kitchen, and spacious outdoor terraces ideal for sunset watching.",
                    Amenities = new List<PropertyAmenity>
                    {
                        new() { Name = "Private pool", IsAvailable = true },
                        new() { Name = "Ocean view", IsAvailable = true },
                        new() { Name = "WiFi", IsAvailable = true },
                        new() { Name = "Air conditioning", IsAvailable = true },
                        new() { Name = "Kitchen", IsAvailable = true },
                        new() { Name = "Free parking", IsAvailable = true }
                    }
                },
                Host = new HostInfo { Name = "Maria", IsSuperhost = true, Rating = 4.98, Reviews = 342 },
                MatchedKeywords = keywords.Take(2).ToList(),
                MatchContexts = new List<string> { $"...breathtaking oceanfront villa! This stunning property offers panoramic {keywords.FirstOrDefault()}..." }
            },
            new()
            {
                Property = new AirbnbProperty
                {
                    Id = "demo2",
                    Title = "Cozy Downtown Loft near Metro Station",
                    Description = $"Modern loft in heart of {location}",
                    Rating = 4.87,
                    Reviews = 89,
                    Price = new PriceInfo { TotalPrice = "$420", ExtractedTotalPrice = 420, Qualifier = "for 5 nights", ExtractedPricePerQualifier = 84 },
                    Accommodations = new List<string> { "1 bedroom", "1 bathroom", "2 guests" },
                    Badges = new List<string> { "Superhost" },
                    Images = new List<string> { "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800" },
                    Link = "https://airbnb.com/rooms/demo2"
                },
                FullDetails = new AirbnbPropertyDetails
                {
                    Title = "Cozy Downtown Loft near Metro Station",
                    Description = $"Perfect urban retreat in the heart of {location}! This stylish loft is just steps from the metro station, making it incredibly easy to explore the city. The space features exposed brick walls, high ceilings, and modern furnishings. Walking distance to top restaurants, cafes, and attractions.",
                    Amenities = new List<PropertyAmenity>
                    {
                        new() { Name = "WiFi", IsAvailable = true },
                        new() { Name = "Air conditioning", IsAvailable = true },
                        new() { Name = "Kitchen", IsAvailable = true },
                        new() { Name = "Washer", IsAvailable = true }
                    }
                },
                Host = new HostInfo { Name = "Carlos", IsSuperhost = true, Rating = 4.92, Reviews = 156 },
                MatchedKeywords = keywords.Take(1).ToList(),
                MatchContexts = new List<string> { $"...just steps from the {keywords.FirstOrDefault()} station, making it incredibly easy..." }
            },
            new()
            {
                Property = new AirbnbProperty
                {
                    Id = "demo3",
                    Title = "Charming Garden Apartment with Terrace",
                    Description = $"Peaceful retreat in {location}",
                    Rating = 4.92,
                    Reviews = 67,
                    Price = new PriceInfo { TotalPrice = "$580", ExtractedTotalPrice = 580, Qualifier = "for 5 nights", ExtractedPricePerQualifier = 116 },
                    Accommodations = new List<string> { "2 bedrooms", "1 bathroom", "4 guests" },
                    Badges = new List<string> { "Guest favorite" },
                    Images = new List<string> { "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=800" },
                    Link = "https://airbnb.com/rooms/demo3"
                },
                FullDetails = new AirbnbPropertyDetails
                {
                    Title = "Charming Garden Apartment with Terrace",
                    Description = $"Escape to this serene garden apartment in a quiet neighborhood of {location}. Features a beautiful private terrace surrounded by lush greenery, perfect for morning coffee or evening relaxation. The apartment has been recently renovated with a modern kitchen and comfortable bedrooms.",
                    Amenities = new List<PropertyAmenity>
                    {
                        new() { Name = "Garden", IsAvailable = true },
                        new() { Name = "Terrace", IsAvailable = true },
                        new() { Name = "WiFi", IsAvailable = true },
                        new() { Name = "Kitchen", IsAvailable = true }
                    }
                },
                Host = new HostInfo { Name = "Sophie", IsSuperhost = false, Rating = 4.88, Reviews = 92 },
                MatchedKeywords = keywords.Take(1).ToList(),
                MatchContexts = new List<string> { $"...beautiful private terrace surrounded by lush {keywords.FirstOrDefault()}..." }
            }
        };
    }
}
