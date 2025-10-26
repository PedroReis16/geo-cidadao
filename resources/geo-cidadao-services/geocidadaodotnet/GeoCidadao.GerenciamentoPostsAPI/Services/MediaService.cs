using GeoCidadao.Cloud.Contracts;
using GeoCidadao.Cloud.Models.BucketRequests;
using GeoCidadao.GerenciamentoPostsAPI.Contracts;

namespace GeoCidadao.GerenciamentoPostsAPI.Services
{
    public class MediaService(ICloudBucketService bucketService) : IMediaService
    {
        private readonly ICloudBucketService _bucketService = bucketService;


        public Task<List<string>> GetPostMediaKeysAsync(Guid postId)
        {
            return _bucketService.ListObjectsAsync(new()
            {

            });
        }

        public Task UploadMediaAsync(Guid postId, Guid mediaId, Stream fileContent, out string fileExtension)
        {
            if (fileContent.Length == 0)
                throw new ArgumentNullException(nameof(fileContent), "O conteúdo do arquivo não pode ser nulo ou vazio");

            byte[] buffer = new byte[8];
            fileContent.Read(buffer, 0, 8);
            fileContent.Position = 0;

            // Check for JPEG (FF D8 FF)
            bool isJpeg = buffer.Take(3).SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF });

            // Check for PNG (89 50 4E 47 0D 0A 1A 0A)
            bool isPng = buffer.Take(8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });

            // Check for MP4 (starts with ftyp at offset 4)
            bool isMp4 = buffer.Length >= 8 && buffer.Skip(4).Take(4).SequenceEqual(new byte[] { 0x66, 0x74, 0x79, 0x70 });

            // Check for GIF (47 49 46 38 37 61 or 47 49 46 38 39 61)
            bool isGif = buffer.Take(6).SequenceEqual(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }) ||
                        buffer.Take(6).SequenceEqual(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 });

            if (!isJpeg && !isPng && !isMp4)
                throw new InvalidOperationException("Formato de mídia inválido. Apenas JPEG, PNG e MP4 são suportados.");

            fileExtension = isJpeg ? ".jpg" : isPng ? ".png" : isGif ? ".gif" : ".mp4";

            PutObjectRequest putRequest = new()
            {
                FileContent = fileContent,
                ObjectKey = $"{postId}/{mediaId}{fileExtension}"
            };

            return _bucketService.PutObjectAsync(putRequest);
        }
        public Task DeleteMediaAsync(string mediaKey)
        {
            return _bucketService.DeleteObjectAsync(new()
            {
                ObjectKey = mediaKey
            });
        }
    }
}