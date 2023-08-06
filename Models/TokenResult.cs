using System.Text.Json.Serialization;

namespace SignerOneIdentityClient.Models;

public class TokenResult
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresInSeconds { get; set; }
}
