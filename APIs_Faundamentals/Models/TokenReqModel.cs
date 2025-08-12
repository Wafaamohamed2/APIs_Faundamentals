using System.ComponentModel.DataAnnotations;

namespace APIs_Faundamentals.Models
{
    public class TokenReqModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
       

    }
}
