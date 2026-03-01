namespace AirbnbKeywordFinder.Core.Models;

/// <summary>
/// Represents search parameters for the Airbnb API.
/// </summary>
public class AirbnbSearchRequest
{
    public string? Query { get; set; }
    public string? BoundingBox { get; set; }
    public string? AirbnbDomain { get; set; }
    public string? Currency { get; set; }
    public DateOnly? CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
    public string? TimePeriod { get; set; }
    public int? Adults { get; set; }
    public int? Children { get; set; }
    public int? Infants { get; set; }
    public int? Pets { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public string? TypeOfPlace { get; set; }
    public string? PropertyTypes { get; set; }
    public int? Bedrooms { get; set; }
    public int? Beds { get; set; }
    public int? Bathrooms { get; set; }
    public string? Amenities { get; set; }
    public string? HostUserId { get; set; }
    public string? PageToken { get; set; }

    public Dictionary<string, string> ToQueryParameters()
    {
        var parameters = new Dictionary<string, string> { ["engine"] = "airbnb" };

        // q is required by the API; when searching by host without a location, send empty string
        parameters["q"] = string.IsNullOrWhiteSpace(Query) ? "" : Query;
        if (!string.IsNullOrWhiteSpace(BoundingBox)) parameters["bounding_box"] = BoundingBox;
        if (!string.IsNullOrWhiteSpace(AirbnbDomain)) parameters["airbnb_domain"] = AirbnbDomain;
        if (!string.IsNullOrWhiteSpace(Currency)) parameters["currency"] = Currency;
        if (CheckInDate.HasValue) parameters["check_in_date"] = CheckInDate.Value.ToString("yyyy-MM-dd");
        if (CheckOutDate.HasValue) parameters["check_out_date"] = CheckOutDate.Value.ToString("yyyy-MM-dd");
        if (!string.IsNullOrWhiteSpace(TimePeriod)) parameters["time_period"] = TimePeriod;
        if (Adults.HasValue) parameters["adults"] = Adults.Value.ToString();
        if (Children.HasValue) parameters["children"] = Children.Value.ToString();
        if (Infants.HasValue) parameters["infants"] = Infants.Value.ToString();
        if (Pets.HasValue) parameters["pets"] = Pets.Value.ToString();
        if (PriceMin.HasValue) parameters["price_min"] = PriceMin.Value.ToString();
        if (PriceMax.HasValue) parameters["price_max"] = PriceMax.Value.ToString();
        if (!string.IsNullOrWhiteSpace(TypeOfPlace)) parameters["type_of_place"] = TypeOfPlace;
        if (!string.IsNullOrWhiteSpace(PropertyTypes)) parameters["property_types"] = PropertyTypes;
        if (Bedrooms.HasValue) parameters["bedrooms"] = Bedrooms.Value.ToString();
        if (Beds.HasValue) parameters["beds"] = Beds.Value.ToString();
        if (Bathrooms.HasValue) parameters["bathrooms"] = Bathrooms.Value.ToString();
        if (!string.IsNullOrWhiteSpace(Amenities)) parameters["amenities"] = Amenities;
        if (!string.IsNullOrWhiteSpace(HostUserId)) parameters["airbnb_user_id"] = HostUserId;
        if (!string.IsNullOrWhiteSpace(PageToken)) parameters["cursor"] = PageToken;

        return parameters;
    }
}
