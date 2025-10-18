namespace GeoCidadao.Cloud.Models.BucketRequests
{
    public class DeleteObjectRequest : BaseBucketRequest
    {
        public string ObjectKey { get; set; } = null!;
    }
}