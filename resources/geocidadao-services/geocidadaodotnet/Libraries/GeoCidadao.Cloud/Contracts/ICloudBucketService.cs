using GeoCidadao.Cloud.Models.BucketRequests;

namespace GeoCidadao.Cloud.Contracts
{
    public interface ICloudBucketService
    {
        Task<List<string>> ListObjectsAsync(ListObjectsRequest request);
        Task PutObjectAsync(PutObjectRequest request);
        Task DeleteObjectAsync(DeleteObjectRequest request);
        Task GetObjectAsync(GetObjectRequest request);
        Task<string> GetPreSignedUrlAsync(GetPreSignedUrlRequest request);
    }
}