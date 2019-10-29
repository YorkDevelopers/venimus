namespace VenimusAPIs.Services.Auth0Models
{
    public class UserProfile
    {
        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
