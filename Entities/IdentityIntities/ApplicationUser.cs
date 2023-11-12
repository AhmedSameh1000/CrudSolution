using Microsoft.AspNetCore.Identity;

namespace Entities.IdentityIntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? PersonName { get; set; }
    }
}