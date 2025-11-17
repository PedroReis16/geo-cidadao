namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs
{
    public class LocationQueryDTO
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusKm { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public int? ItemsCount { get; set; }
        public int? PageNumber { get; set; }
    }
}
