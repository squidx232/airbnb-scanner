using System.Text.Json.Serialization;

namespace AirbnbKeywordFinder.Core.Models;

public class AirbnbPropertyResponse
{
    [JsonPropertyName("search_metadata")] public SearchMetadata? SearchMetadata { get; set; }
    [JsonPropertyName("property")] public AirbnbPropertyDetails? Property { get; set; }
    [JsonPropertyName("host")] public HostInfo? Host { get; set; }
    [JsonPropertyName("error")] public string? Error { get; set; }
}

public class AirbnbPropertyDetails
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("link")] public string? Link { get; set; }
    [JsonPropertyName("rating")] public double? Rating { get; set; }
    [JsonPropertyName("reviews")] public int? Reviews { get; set; }
    [JsonPropertyName("guest_capacity")] public int? GuestCapacity { get; set; }
    [JsonPropertyName("highlights")] public List<PropertyHighlight> Highlights { get; set; } = new();
    [JsonPropertyName("accomodations")] public List<string> Accommodations { get; set; } = new();
    [JsonPropertyName("amenities")] public List<PropertyAmenity> Amenities { get; set; } = new();
    [JsonPropertyName("house_rules")] public HouseRules? HouseRules { get; set; }
    [JsonPropertyName("gps_coordinates")] public GpsCoordinates? GpsCoordinates { get; set; }
    [JsonPropertyName("images")] public List<PropertyImage> Images { get; set; } = new();

    public string GetSearchableText()
    {
        var parts = new List<string?>
        {
            Title, Description,
            string.Join(" ", Accommodations),
            string.Join(" ", Highlights.Select(h => $"{h.Name} {h.Description}")),
            string.Join(" ", Amenities.Where(a => a.IsAvailable == true).Select(a => a.Name)),
            HouseRules?.DuringYourStay != null ? string.Join(" ", HouseRules.DuringYourStay) : null
        };
        return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }
}

public class PropertyHighlight
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
}

public class PropertyAmenity
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("is_available")] public bool? IsAvailable { get; set; }
}

public class HouseRules
{
    [JsonPropertyName("check_in")] public string? CheckIn { get; set; }
    [JsonPropertyName("check_out")] public string? CheckOut { get; set; }
    [JsonPropertyName("during_your_stay")] public List<string> DuringYourStay { get; set; } = new();
}

public class PropertyImage
{
    [JsonPropertyName("caption")] public string? Caption { get; set; }
    [JsonPropertyName("link")] public string? Link { get; set; }
}

public class HostInfo
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("about")] public string? About { get; set; }
    [JsonPropertyName("is_superhost")] public bool? IsSuperhost { get; set; }
    [JsonPropertyName("is_verified")] public bool? IsVerified { get; set; }
    [JsonPropertyName("rating")] public double? Rating { get; set; }
    [JsonPropertyName("reviews")] public int? Reviews { get; set; }
    [JsonPropertyName("response_rate")] public string? ResponseRate { get; set; }
    [JsonPropertyName("avatar")] public string? Avatar { get; set; }
}
