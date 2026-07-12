  using Microsoft.AspNetCore.Identity;

namespace eCommerceApp.Domain.Entities.Identity
{
    public class AppUser : IdentityUser
    {
       public string FullName { get; set; } = string.Empty;
      public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? ImageUrl { get; set; }
    
    }
}
