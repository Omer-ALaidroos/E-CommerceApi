

using eCommerceApp.Domain.Entities.Identity;

public class Address
{
    public int Id { get; set; }
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required bool IsActive { get; set; } = true;
    public required string UserId { get; set; } // Foreign key to User
    public required AppUser User { get; set; }

    
}



