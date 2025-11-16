namespace GeoCidadao.FeedServiceAPI.Config
{
    /// <summary>
    /// Configuração de URLs dos serviços externos
    /// </summary>
    public class ExternalServicesConfig
    {
        public string PostsApiUrl { get; set; } = string.Empty;
        public string UsersApiUrl { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
    }
}
