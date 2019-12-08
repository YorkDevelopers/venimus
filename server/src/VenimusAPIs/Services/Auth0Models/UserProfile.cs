namespace VenimusAPIs.Services.Auth0Models
{
    public class UserProfile
    {
        [System.Text.Json.Serialization.JsonPropertyName("email")]
        [Newtonsoft.Json.JsonProperty("email")]
        public string Email { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("given_name")]
        [Newtonsoft.Json.JsonProperty("given_name")]
        public string GivenName { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("family_name")]
        [Newtonsoft.Json.JsonProperty("family_name")]
        public string FamilyName { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("nick_name")]
        [Newtonsoft.Json.JsonProperty("nick_name")]
        public string NickName { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("picture")]
        [Newtonsoft.Json.JsonProperty("picture")]
        public string Picture { get; set; } = default!;
    }
}
