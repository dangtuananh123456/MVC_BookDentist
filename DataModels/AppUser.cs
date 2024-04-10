using Microsoft.AspNetCore.Identity;

namespace DataModels
{
	public class AppUser : IdentityUser
	{
        public bool IsLocked { get; set; } = false;
    }
}
