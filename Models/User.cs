using Microsoft.AspNetCore.Identity;

namespace AppIdentity.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}