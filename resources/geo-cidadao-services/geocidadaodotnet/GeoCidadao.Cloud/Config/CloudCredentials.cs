using Amazon.Runtime;

namespace GeoCidadao.Cloud.Config
{
    public class CloudCredentials
    {
        public string Region { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;

        

        public BasicAWSCredentials GetCredentials()
        {
            if (string.IsNullOrWhiteSpace(AccessKey) || string.IsNullOrWhiteSpace(SecretKey))
                throw new ArgumentException("AccessKey and SecretKey must be provided for BasicAWSCredentials");
            return new BasicAWSCredentials(AccessKey, SecretKey);
        }
    }
}