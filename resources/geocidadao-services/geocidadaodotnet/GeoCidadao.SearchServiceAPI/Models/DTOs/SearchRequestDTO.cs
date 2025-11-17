namespace GeoCidadao.SearchServiceAPI.Models.DTOs;

/// <summary>
/// Search request parameters
/// </summary>
public class SearchRequestDTO
{
    /// <summary>
    /// Search query text
    /// </summary>
    public string? Query { get; set; }
    
    /// <summary>
    /// Filter by city/location
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Filter by author ID
    /// </summary>
    public Guid? AuthorId { get; set; }
    
    /// <summary>
    /// Filter by category
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// Filter by date range (ISO 8601 format)
    /// </summary>
    public DateTime? DateFrom { get; set; }
    
    /// <summary>
    /// Filter by date range (ISO 8601 format)
    /// </summary>
    public DateTime? DateTo { get; set; }
    
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Results per page
    /// </summary>
    public int PageSize { get; set; } = 20;
    
    /// <summary>
    /// Search type: posts, users, or all
    /// </summary>
    public string SearchType { get; set; } = "posts";
}
