using System.ComponentModel.DataAnnotations;

namespace APIs_Faundamentals.DTO
{
    public class UserDataDTO
    {

        [Required ,StringLength(100) ]
        public string FirstName { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string UserName { get; set; } = string.Empty;
        [Required, StringLength(128)]
        public string Password { get; set; } = string.Empty;
        [Required, StringLength(256)]
        public string Email { get; set; } = string.Empty;
        
    }
}
