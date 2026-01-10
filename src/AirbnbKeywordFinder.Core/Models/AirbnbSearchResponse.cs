using System.Text.Json.Serialization;

namespace AirbnbKeywordFinder.Core.Models;

public class AirbnbSearchResponse
{
    [JsonPropertyName("search_metadata")]
    public SearchMetadata? SearchMetadata { get; set; }

    [JsonPropertyName("search_parameters")]
    public SearchParameters? SearchParameters { get; set; }

    [JsonPropertyName("search_information")]
    public SearchInformation? SearchInformation { get; set; }

    [JsonPropertyName("properties")]
    public List<AirbnbProperty> Properties { get; set; } = new();

    [JsonPropertyName("pagination")]
    public Pagination? Pagination { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}

public class SearchMetadata
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("created_at")] public string? CreatedAt { get; set; }
    [JsonPropertyName("request_time_taken")] public double? RequestTimeTaken { get; set; }
    [JsonPropertyName("total_time_taken")] public double? TotalTimeTaken { get; set; }
    [JsonPropertyName("request_url")] public string? RequestUrl { get; set; }
}

public class SearchParameters
{
    [JsonPropertyName("engine")] public string? Engine { get; set; }
    [JsonPropertyName("airbnb_domain")] public string? AirbnbDomain { get; set; }
    [JsonPropertyName("q")] public string? Query { get; set; }
    [JsonPropertyName("check_in_date")] public string? CheckInDate { get; set; }
    [JsonPropertyName("check_out_date")] public string? CheckOutDate { get; set; }
    [JsonPropertyName("adults")] public string? Adults { get; set; }
    [JsonPropertyName("time_period")] public string? TimePeriod { get; set; }
}

public class SearchInformation
{
    [JsonPropertyName("query_displayed")] public string? QueryDisplayed { get; set; }
    [JsonPropertyName("results")] public string? Results { get; set; }
    [JsonPropertyName("time_period")] public string? TimePeriod { get; set; }
    [JsonPropertyName("check_in_date")] public string? CheckInDate { get; set; }
    [JsonPropertyName("check_out_date")] public string? CheckOutDate { get; set; }
    [JsonPropertyName("guests")] public string? Guests { get; set; }
    [JsonPropertyName("adults")] public int? Adults { get; set; }
}

public class AirbnbProperty
{
    [JsonPropertyName("position")] public int Position { get; set; }
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("link")] public string? Link { get; set; }
    [JsonPropertyName("booking_link")] public string? BookingLink { get; set; }
    [JsonPropertyName("rating")] public double? Rating { get; set; }
    [JsonPropertyName("reviews")] public int? Reviews { get; set; }
    [JsonPropertyName("price")] public PriceInfo? Price { get; set; }
    [JsonPropertyName("accommodations")] public List<string> Accommodations { get; set; } = new();
    [JsonPropertyName("gps_coordinates")] public GpsCoordinates? GpsCoordinates { get; set; }
    [JsonPropertyName("badges")] public List<string> Badges { get; set; } = new();
    [JsonPropertyName("images")] public List<string> Images { get; set; } = new();
    [JsonPropertyName("time_period")] public string? TimePeriod { get; set; }
    [JsonPropertyName("check_in_date")] public string? CheckInDate { get; set; }
    [JsonPropertyName("check_out_date")] public string? CheckOutDate { get; set; }

    public string GetSearchableText() =>
        string.Join(" ", new[] { Title, Description, string.Join(" ", Accommodations), string.Join(" ", Badges) }
            .Where(p => !string.IsNullOrWhiteSpace(p)));
}

public class PriceInfo
{
    [JsonPropertyName("total_price")] public string? TotalPrice { get; set; }
    [JsonPropertyName("extracted_total_price")] public decimal? ExtractedTotalPrice { get; set; }
    [JsonPropertyName("qualifier")] public string? Qualifier { get; set; }
    [JsonPropertyName("extracted_qualifier")] public int? ExtractedQualifier { get; set; }
    [JsonPropertyName("original_price")] public string? OriginalPrice { get; set; }
    [JsonPropertyName("extracted_original_price")] public decimal? ExtractedOriginalPrice { get; set; }
    [JsonPropertyName("price_per_qualifier")] public string? PricePerQualifier { get; set; }
    [JsonPropertyName("extracted_price_per_qualifier")] public decimal? ExtractedPricePerQualifier { get; set; }
}

public class GpsCoordinates
{
    [JsonPropertyName("latitude")] public double Latitude { get; set; }
    [JsonPropertyName("longitude")] public double Longitude { get; set; }
}

public class Pagination
{
    [JsonPropertyName("next_page_token")] public string? NextPageToken { get; set; }
}
