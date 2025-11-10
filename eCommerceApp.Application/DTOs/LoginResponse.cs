namespace eCommerceApp.Application.DTOs
{
    public record LoginResponse(
        bool Success=false,
         string Message = null!,
        string? Token = null,
        string? Refreshtoken = null);
}
