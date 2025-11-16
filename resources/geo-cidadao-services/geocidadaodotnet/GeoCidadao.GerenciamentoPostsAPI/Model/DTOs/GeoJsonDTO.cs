namespace GeoCidadao.GerenciamentoPostsAPI.Model.DTOs
{
    public class GeoJsonFeatureCollectionDTO
    {
        public string Type { get; set; } = "FeatureCollection";
        public List<GeoJsonFeatureDTO> Features { get; set; } = new();
    }

    public class GeoJsonFeatureDTO
    {
        public string Type { get; set; } = "Feature";
        public GeoJsonGeometryDTO Geometry { get; set; } = null!;
        public object Properties { get; set; } = null!;
    }

    public class GeoJsonGeometryDTO
    {
        public string Type { get; set; } = "Point";
        public double[] Coordinates { get; set; } = null!; // [longitude, latitude]
    }

    public class PostMapPropertiesDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }
}
