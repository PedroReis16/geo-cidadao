namespace GeoCidadao.Cloud.Models.BucketRequests
{
    public abstract class BaseBucketRequest
    {
        public string BucketName { get; set; } = null!;
        public string Region { get; set; } = null!;
    }
}