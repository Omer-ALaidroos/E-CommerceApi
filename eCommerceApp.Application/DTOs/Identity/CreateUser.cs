namespace eCommerceApp.Application.DTOs
{

    public class CreateUser :BaseModel
    {
        public required string FullName { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
