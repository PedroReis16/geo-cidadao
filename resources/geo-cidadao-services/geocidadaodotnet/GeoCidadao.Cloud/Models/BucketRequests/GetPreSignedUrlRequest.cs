
namespace GeoCidadao.Cloud.Models.BucketRequests
{
    public class GetPreSignedUrlRequest : BaseBucketRequest
    {
        public string ObjectKey { get; set; } = null!;
    }
}