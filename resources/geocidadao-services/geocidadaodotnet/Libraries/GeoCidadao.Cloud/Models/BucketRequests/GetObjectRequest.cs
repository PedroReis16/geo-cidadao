

namespace GeoCidadao.Cloud.Models.BucketRequests
{
    public class GetObjectRequest 
    {
        public string ObjectKey { get; set; } = null!;
        public bool DoesSaveFile { get; set; } = true;

        private string? _filePath;
        public string? FilePath
        {
            get => string.IsNullOrWhiteSpace(_filePath) ? ObjectKey : _filePath;
            set => _filePath = value;
        }
    }
}