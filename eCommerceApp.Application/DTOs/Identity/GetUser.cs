namespace eCommerceApp.Application.DTOs.Identity
{
    public class GetUser
    {
        public required string Id { get; set; }

        public string ? ImageUrl { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role {  get; set; }
    }
}
