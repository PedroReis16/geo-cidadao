using GeoCidadao.Models.Enums;

namespace GeoCidadao.SearchServiceAPI.Models;

/// <summary>
/// Elasticsearch document for Post indexing
/// </summary>
public class PostDocument
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public PostCategory Category { get; set; }
    public List<string> Tags { get; set; } = new();
    
    // Location information
    public string? LocationCity { get; set; }
    public string? LocationNeighborhood { get; set; }
    public double? LocationLatitude { get; set; }
    public double? LocationLongitude { get; set; }
    
    // Interaction metrics for relevance
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public double RelevanceScore { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Status flags
    public bool IsDeleted { get; set; }
    public bool IsPublic { get; set; } = true;
}
