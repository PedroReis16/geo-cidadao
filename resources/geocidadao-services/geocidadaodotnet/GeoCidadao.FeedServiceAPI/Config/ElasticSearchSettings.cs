namespace GeoCidadao.FeedServiceAPI.Config
{
    public class ElasticSearchSettings
    {
        public string Uri { get; set; } = string.Empty;
        public string DefaultIndex { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
