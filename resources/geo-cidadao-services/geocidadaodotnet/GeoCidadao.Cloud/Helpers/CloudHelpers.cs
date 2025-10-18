using GeoCidadao.Cloud.Config;
using Microsoft.Extensions.Configuration;

namespace GeoCidadao.Cloud.Helpers
{
    public class CloudHelpers
    {
        internal static CloudCredentials GetAwsCredentials(IConfiguration configuration)
        {
            CloudCredentials? credentials = configuration.GetSection("AWSCredentials").Get<CloudCredentials>();

            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials), "As credenciais do bucket não foram informadas");

            return credentials;
        }
    }
}