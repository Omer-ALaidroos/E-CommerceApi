
namespace eCommerceApp.Application.DTOs.Address
{
    public class CreateAddress
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required Guid UserId { get; set; }
    }
}
