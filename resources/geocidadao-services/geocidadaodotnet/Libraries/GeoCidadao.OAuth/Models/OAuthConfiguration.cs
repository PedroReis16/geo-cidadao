namespace GeoCidadao.OAuth.Models;

public class OAuthConfiguration
{
    public string Authority { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public Dictionary<string, string> ClaimRoles { get; set; } = new();
    public Dictionary<string, string> GroupClaims { get; set; } = new();
}
