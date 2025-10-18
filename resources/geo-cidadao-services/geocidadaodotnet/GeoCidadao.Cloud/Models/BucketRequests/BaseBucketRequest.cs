namespace GeoCidadao.Cloud.Models.BucketRequests
{
    public abstract class BaseBucketRequest
    {
        public string BucketName { get; set; } = null!;
        public string? Namespace { get; set; }
        public string Region { get; set; } = null!;
    }
}