using Microsoft.EntityFrameworkCore;

namespace APIs_Faundamentals.Models
{

    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }

        public DateTime ExpireOn { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpireOn;

        public DateTime CreateOn { get; set; }

        public DateTime? RevokeedOn { get; set; }

        public bool IsAtive => RevokeedOn == null && !IsExpired;

    }
}
