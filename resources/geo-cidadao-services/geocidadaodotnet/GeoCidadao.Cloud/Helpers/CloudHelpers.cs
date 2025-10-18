using GeoCidadao.Cloud.Models;
using Microsoft.Extensions.Configuration;

namespace GeoCidadao.Cloud.Helpers
{
    public class CloudHelpers
    {
        internal static BucketCredentials GetAwsCredentials(IConfiguration configuration)
        {
            BucketCredentials? credentials = configuration.GetSection("AWSCredentials").Get<BucketCredentials>();

            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials), "As credenciais do bucket não foram informadas");

            return credentials;
        }
    }
}