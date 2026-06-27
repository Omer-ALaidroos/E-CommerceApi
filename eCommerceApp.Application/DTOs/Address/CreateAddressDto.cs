
namespace eCommerceApp.Application.DTOs.Address
{
    public class CreateAddress
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public  string? UserId { get; set; }
    }
}
