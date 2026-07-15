public class TestData
{
    public HospitalSearchCriteria HospitalSearch { get; set; } = new();
}

public class HospitalSearchCriteria
{
    public string City { get; set; } = string.Empty;
    public double MinRating { get; set; }
    public double MaxRating { get; set; }
    public bool RequireOpen24Hours { get; set; }
    public bool RequireParking { get; set; }
}