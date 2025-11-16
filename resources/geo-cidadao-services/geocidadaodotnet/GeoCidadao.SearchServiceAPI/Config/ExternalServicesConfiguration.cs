namespace GeoCidadao.SearchServiceAPI.Config;

public class ExternalServicesConfiguration
{
    public string PostsApiUrl { get; set; } = string.Empty;
    public string UsersApiUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}
