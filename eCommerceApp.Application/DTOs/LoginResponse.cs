namespace eCommerceApp.Application.DTOs
{
    public record LoginResponse(
        string? UserId=null,
        bool Success=false,
         string Message = null!,
        string? Token = null,
        string? Refreshtoken = null);
}
