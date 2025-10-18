using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using GeoCidadao.Cloud.Config;

namespace GeoCidadao.Cloud.Models
{
    internal class BucketCredentials : CloudCredentials
    {
        public string BucketName { get; set; } = null!;

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