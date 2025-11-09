using NetTopologySuite.Geometries;
using GeoCidadao.Model.Entities;
using GeoCidadao.Model.Enums;

namespace GeoCidadao.Database.Entities.GerenciamentoPostsAPI
{
    public class PostLocation : BaseEntity
    {
        public Guid PostId { get; set; }
        public Point Position { get; set; } = null!;
        public PostCategory Category { get; set; }
    }
}