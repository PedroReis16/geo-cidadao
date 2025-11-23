using GeoCidadao.Models.Entities;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using NetTopologySuite.Geometries;

namespace GeoCidadao.Database.Entities.GerenciamentoPostsAPI
{
    public class PostLocation : BaseEntity
    {
        public Point Location { get; set; } = null!;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Suburb { get; set; } = string.Empty;

        public Post Post { get; set; } = null!;
    }
}