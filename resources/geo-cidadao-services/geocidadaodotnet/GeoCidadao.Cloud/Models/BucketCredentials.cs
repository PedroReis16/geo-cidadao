using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using GeoCidadao.Cloud.Config;

namespace GeoCidadao.Cloud.Models
{
    internal class BucketCredentials : CloudCredentials
    {
        public string BucketName { get; set; } = null!;

        public BucketCredentials(string bucketName, CloudCredentials credentials)
        {
            BucketName = bucketName;
            AccessKey = credentials.AccessKey;
            SecretKey = credentials.SecretKey;
            ServiceURL = credentials.ServiceURL;
            Region = credentials.Region;
        }

        public AmazonS3Client GetClient()
        {
            return new AmazonS3Client(new BasicAWSCredentials("test", "test"), new AmazonS3Config
            {
                ServiceURL = ServiceURL, // endpoint from localstack
                ForcePathStyle = true, // for localstack compatibility
                UseHttp = ServiceURL.StartsWith("http://", StringComparison.OrdinalIgnoreCase),
                AuthenticationRegion = Region
            });
        }
    }
}