using Amazon.S3;
using GeoCidadao.Cloud.Config;

namespace GeoCidadao.Cloud.Models
{
    internal class BucketCredentials : CloudCredentials
    {
        public string BucketName { get; set; } = null!;

        public BucketCredentials(string bucketName, string region, CloudCredentials credentials)
        {
            BucketName = bucketName;
            AccessKey = credentials.AccessKey;
            SecretKey = credentials.SecretKey;
            Region = region;
        }

        public AmazonS3Config GetBucketConfig()
        {
            return new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(Region)
            };
        }

        public AmazonS3Client GetClient()
        {
            if (string.IsNullOrEmpty(BucketName))
                throw new ArgumentException("Bucket name cannot be null or empty.", nameof(BucketName));

            return new AmazonS3Client(GetCredentials(), GetBucketConfig());
        }
    }
}