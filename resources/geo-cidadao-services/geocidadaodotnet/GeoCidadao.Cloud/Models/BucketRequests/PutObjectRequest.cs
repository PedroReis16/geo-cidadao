
namespace GeoCidadao.Cloud.Models.BucketRequests
{
    public class PutObjectRequest 
    {
        public Stream FileContent { get; set; } = null!;
        public string ObjectKey { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }

        public override string ToString() => ObjectKey;
    }
}