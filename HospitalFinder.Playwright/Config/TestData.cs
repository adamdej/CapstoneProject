public class TestData
{
    public DiagnosticsData Diagnostics { get; set; } = new();
    public CorporateWellnessFormData CorporateWellnessForm { get; set; } = new();
}

public class DiagnosticsData
{
    public int ExpectedTopCityCount { get; set; }
    public List<string> ExpectedCities { get; set; } = new();
}

public class CorporateWellnessFormData
{
    public string InvalidContactNumber { get; set; } = string.Empty;
    public string ValidContactNumber { get; set; } = string.Empty;
    public string InvalidEmail { get; set; } = string.Empty;
    public string ValidEmail { get; set; } = string.Empty;
}