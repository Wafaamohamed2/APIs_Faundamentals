using System.Text.Json.Serialization;

namespace APIs_Faundamentals.Models
{
    public class AuthModel
    {

        public string Message { get; set; } = string.Empty;

        public bool IsAuthenticated { get; set; } = true;
       
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; }
        public string Email { get; set; } = string.Empty;
      
        public string Token { get; set; } = string.Empty;

        //public DateTime Expiration { get; set; } = DateTime.Now.AddMinutes(30);

        [JsonIgnore]
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpired { get; set; }
    }
}
