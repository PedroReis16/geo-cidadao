namespace GeoCidadao.FeedMapAPI.Middlewares
{
    public sealed class KeycloakAdminOptions
    {
        public string BaseUrl { get; set; } = default!;
        public string Realm { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public int TokenSkewSeconds { get; set; } = 30;
    }
}