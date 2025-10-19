using System.IO.Compression;

namespace GeoCidadao.Model.Helpers
{
    public static class FileHelpers
    {
        public static string GetFileHash(Stream fileContent)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();

            // Certifique-se de que o stream está na posição inicial
            if (fileContent.CanSeek)
                fileContent.Position = 0;

            // Calcula o hash diretamente do stream (sem precisar copiar)
            byte[] hashBytes = sha256.ComputeHash(fileContent);

            // Reseta a posição do stream após leitura
            if (fileContent.CanSeek)
                fileContent.Position = 0;

            // Retorna o hash em formato hexadecimal
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        public static string ExtractZipFile(string sourcePath, string destinationDirectory)
        {
            if (string.IsNullOrEmpty(sourcePath) || !sourcePath.EndsWith(".zip"))
                throw new ArgumentException("Arquivo inválido", nameof(sourcePath));

            if (!File.Exists(sourcePath))
                throw new FileNotFoundException("Arquivo ZIP não encontrado", sourcePath);

            string extractDirectoryName = Path.Combine(destinationDirectory, Path.GetFileNameWithoutExtension(sourcePath));

            if (Directory.Exists(extractDirectoryName))
            {
                Directory.Delete(extractDirectoryName, true);
            }

            Directory.CreateDirectory(extractDirectoryName);
            ZipFile.ExtractToDirectory(sourcePath, extractDirectoryName);


            return extractDirectoryName;
        }
    }
}