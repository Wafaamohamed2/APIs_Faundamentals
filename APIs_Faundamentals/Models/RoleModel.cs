using System.ComponentModel.DataAnnotations;

namespace APIs_Faundamentals.Models
{
    public class RoleModel
    {
        [Required]

        public string Id { get; set; }

        [Required, StringLength(256)]
        public string Role { get; set; } = string.Empty;
    }
}
