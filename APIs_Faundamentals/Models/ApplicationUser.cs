using Microsoft.AspNetCore.Identity;

namespace APIs_Faundamentals.Models
{
    public class ApplicationUser : IdentityUser

    {


        public string FirstName { get; set; }
        public string LastName { get; set; }

       


    }
}
