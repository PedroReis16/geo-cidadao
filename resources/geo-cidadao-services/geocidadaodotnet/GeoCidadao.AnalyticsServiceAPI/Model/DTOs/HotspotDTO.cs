namespace GeoCidadao.AnalyticsServiceAPI.Model.DTOs
{
    public class HotspotDTO
    {
        public string RegionIdentifier { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int PostCount { get; set; }
        public double HeatScore { get; set; } // Normalized score for heat mapping
    }
}
