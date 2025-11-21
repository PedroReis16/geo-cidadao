using Microsoft.Extensions.Configuration;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;
using Amazon.S3;
using Amazon.Runtime;

namespace MobilePacs.TestShared.Containers
{
    public class LocalStackFixture : IAsyncLifetime
    {
        public IContainer Container { get; private set; } = default!;
        public IAmazonS3 S3Client { get; private set; } = default!;
        public IConfiguration Configuration { get; private set; } = default!;

        private const string BucketName = "updates-bucket";

        public async Task InitializeAsync()
        {
            Container = new ContainerBuilder()
                .WithImage("localstack/localstack:latest")
                .WithEnvironment("SERVICES", "s3")
                .WithEnvironment("AWS_DEFAULT_REGION", "us-east-1")
                .WithPortBinding(4566, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilExternalTcpPortIsAvailable(4566))
                .Build();

            await Container.StartAsync();

            var serviceUrl = $"http://localhost:{Container.GetMappedPublicPort(4566)}";

            S3Client = new AmazonS3Client(new BasicAWSCredentials("test", "test"), new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true,
                UseHttp = serviceUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase),
                AuthenticationRegion = "us-east-1"
            });

            // Configuração usada no serviço
            Dictionary<string, string> settings = new Dictionary<string, string>
            {
                { "CheckUpdateS3Config:S3BucketName", BucketName }
            };
            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings!)
                .Build();

            // Cria bucket inicial
            await S3Client.PutBucketAsync(BucketName);
        }

        public async Task DisposeAsync()
        {
            await Container.StopAsync();
            await Container.DisposeAsync();
        }
    }
}