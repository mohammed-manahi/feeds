using Microsoft.AspNetCore.Identity;

namespace Feeds.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public byte[] ProfilePicture { get; set; }
}