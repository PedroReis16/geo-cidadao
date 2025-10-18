using System.Buffers;
using System.Collections.Concurrent;
using System.Threading.Channels;
using Amazon.S3;
using Amazon.S3.Model;
using GeoCidadao.Cloud.Contracts;
using GeoCidadao.Cloud.Helpers;
using GeoCidadao.Cloud.Models;
using Microsoft.Extensions.Configuration;

namespace GeoCidadao.Cloud.Services
{
    public class CloudBucketService(IConfiguration configuration) : ICloudBucketService
    {
        private readonly IConfiguration _configuration = configuration;

        private BucketCredentials GetCredentials() =>
        CloudHelpers.GetAwsCredentials(_configuration);

        public Task<List<string>> ListObjectsAsync(Models.BucketRequests.ListObjectsRequest request)
        {
            BucketCredentials credentials = GetCredentials();

            using AmazonS3Client client = credentials.GetClient();

            ListObjectsV2Request amazonRequest = new()
            {
                BucketName = credentials.BucketName,
            };
            ListObjectsV2Response response = client.ListObjectsV2Async(amazonRequest).Result;
            return Task.FromResult(response.S3Objects.Select(o => o.Key).ToList());
        }

        public async Task GetObjectAsync(Models.BucketRequests.GetObjectRequest request)
        {
            BucketCredentials credentials = GetCredentials();

            using AmazonS3Client client = credentials.GetClient();

            Amazon.S3.Model.GetObjectRequest amazonRequest = new()
            {
                BucketName = credentials.BucketName,
                Key = request.ObjectKey,
            };

            GetObjectResponse response = await client.GetObjectAsync(amazonRequest);

            if (request.DoesSaveFile)
            {
                var directory = Path.GetDirectoryName(request.FilePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                await response.WriteResponseStreamToFileAsync(request.FilePath, false, default);
            }
        }
        
        public Task PutObjectAsync(Models.BucketRequests.PutObjectRequest request)
        {
            BucketCredentials credentials = GetCredentials();

            using AmazonS3Client client = credentials.GetClient();

            Amazon.S3.Model.PutObjectRequest amazonRequest = new()
            {
                BucketName = credentials.BucketName,
                Key = request.ObjectKey,
                InputStream = request.FileContent,
            };

            return client.PutObjectAsync(amazonRequest);
        }

        // public async Task PutObjectAsync(Models.BucketRequests.PutObjectRequest request)
        // {
        //     BucketCredentials credentials = GetCredentials();

        //     using AmazonS3Client client = credentials.GetClient();

        //     InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest
        //     {
        //         BucketName = request.BucketName,
        //         Key = request.ToString(),
        //         StorageClass = S3StorageClass.Standard,
        //     };

        //     InitiateMultipartUploadResponse initiateResponse = await client.InitiateMultipartUploadAsync(initiateRequest);
        //     string uploadId = initiateResponse.UploadId;

        //     const int partSize = 10 * 1024 * 1024; // mínimo S3 = 5 MB (exceto última)
        //     const int maxParallel = 6;
        //     const int channelCapacity = maxParallel * 2;

        //     ArrayPool<byte> pool = ArrayPool<byte>.Shared;
        //     Channel<(int part, byte[] buffer, int count)> channel = Channel.CreateBounded<(int part, byte[] buffer, int count)>(
        //         new BoundedChannelOptions(channelCapacity) { SingleWriter = true, SingleReader = false });
        //     ConcurrentBag<PartETag> partETags = new ConcurrentBag<PartETag>();

        //     try
        //     {

        //         Task producer = Task.Run(async () =>
        //         {
        //             Stream input = request.FileContent ?? throw new ArgumentException("FileContent não informado.");
        //             if (input.CanSeek) input.Seek(0, SeekOrigin.Begin);

        //             int partNum = 1;
        //             while (true)
        //             {
        //                 var buffer = pool.Rent(partSize);
        //                 int bytesRead = await input.ReadAsync(buffer.AsMemory(0, partSize));
        //                 if (bytesRead <= 0)
        //                 {
        //                     pool.Return(buffer);
        //                     break;
        //                 }
        //                 await channel.Writer.WriteAsync((partNum++, buffer, bytesRead));
        //             }
        //             channel.Writer.Complete();
        //         });

        //         Task[] consumers = Enumerable.Range(0, maxParallel).Select(async _ =>
        //         {
        //             ChannelReader<(int part, byte[] buffer, int count)> reader = channel.Reader;
        //             while (await reader.WaitToReadAsync())
        //             {
        //                 while (reader.TryRead(out var item))
        //                 {
        //                     var (partNum, buffer, count) = item;
        //                     try
        //                     {
        //                         using var partStream = new MemoryStream(buffer, 0, count, writable: false);
        //                         var uploadRequest = new UploadPartRequest
        //                         {
        //                             BucketName = credentials.BucketName,
        //                             Key = request.ToString(),
        //                             UploadId = uploadId,
        //                             PartNumber = partNum,
        //                             PartSize = count, // importante: usar bytes reais lidos
        //                             InputStream = partStream,
        //                             UseChunkEncoding = false,
        //                         };

        //                         var resp = await client.UploadPartAsync(uploadRequest);
        //                         partETags.Add(new PartETag(resp.PartNumber, resp.ETag));
        //                     }
        //                     finally
        //                     {
        //                         pool.Return(buffer);
        //                     }
        //                 }
        //             }
        //         }).ToArray();

        //         await producer;
        //         await Task.WhenAll(consumers);

        //         await client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest
        //         {
        //             BucketName = credentials.BucketName,
        //             Key = request.ToString(),
        //             UploadId = uploadId,
        //             PartETags = partETags.OrderBy(p => p.PartNumber).ToList()
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         await client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
        //         {
        //             BucketName = credentials.BucketName,
        //             Key = request.ToString(),
        //             UploadId = uploadId
        //         });

        //         throw new Exception(ex.Message, ex);
        //     }
        // }

        public Task<string> GetPreSignedUrlAsync(Models.BucketRequests.GetPreSignedUrlRequest request)
        {
            BucketCredentials credentials = GetCredentials();

            using AmazonS3Client client = credentials.GetClient();

            GetPreSignedUrlRequest amazonRequest = new()
            {
                BucketName = credentials.BucketName,
                Key = request.ObjectKey,
                Expires = DateTime.Now.AddHours(1),
                Verb = HttpVerb.GET
            };

            string url = client.GetPreSignedURL(amazonRequest);
            return Task.FromResult(url);
        }

        public Task DeleteObjectAsync(Models.BucketRequests.DeleteObjectRequest request)
        {
            BucketCredentials credentials = GetCredentials();

            using AmazonS3Client client = credentials.GetClient();

            Amazon.S3.Model.DeleteObjectRequest amazonRequest = new()
            {
                BucketName = credentials.BucketName,
                Key = request.ObjectKey,
            };

            return client.DeleteObjectAsync(amazonRequest);
        }


    }
}