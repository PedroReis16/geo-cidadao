namespace GeoCidadao.SearchServiceAPI.Config;

public class ElasticsearchConfiguration
{
    public string Url { get; set; } = string.Empty;
    public string DefaultIndex { get; set; } = string.Empty;
}
