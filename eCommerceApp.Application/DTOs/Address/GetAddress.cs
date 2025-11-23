
namespace eCommerceApp.Application.DTOs.Address
{
    public class GetAddress
    {
        public required int Id { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
    }
}
