namespace GeoCidadao.SearchServiceAPI.Models.DTOs;

/// <summary>
/// Search response with results and pagination
/// </summary>
public class SearchResponseDTO<T>
{
    public List<T> Results { get; set; } = new();
    public long TotalResults { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public long SearchTimeMs { get; set; }
}
