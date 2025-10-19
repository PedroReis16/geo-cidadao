using Amazon.S3;
using Amazon.S3.Util;
using GeoCidadao.Cloud.Helpers;
using GeoCidadao.Cloud.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoCidadao.Cloud.Config
{
    public class BucketStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                using IServiceScope scope = app.ApplicationServices.CreateScope();

                IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                BucketCredentials credentials = CloudHelpers.GetAwsCredentials(configuration);

                // Ensure the bucket exists
                using AmazonS3Client client = credentials.GetClient();

                var result = AmazonS3Util.DoesS3BucketExistV2Async(client, credentials.BucketName).GetAwaiter().GetResult();

                if (!result)
                {
                    client.PutBucketAsync(new Amazon.S3.Model.PutBucketRequest
                    {
                        BucketName = credentials.BucketName,
                        UseClientRegion = true
                    }).Wait();
                }

                next(app);
            };
        }

    }
}