namespace GeoCidadao.SearchServiceAPI.Models;

/// <summary>
/// Elasticsearch document for User indexing
/// </summary>
public class UserDocument
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Status flags
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; } = true;
}
