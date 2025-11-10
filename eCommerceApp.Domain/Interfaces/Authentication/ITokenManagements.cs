using System.Security.Claims;

namespace eCommerceApp.Domain.Interfaces.Authentication
{
    public interface ITokenManagements
    {
        string GetRefreshToken();
        List<Claim> GetUserClaimsFromToken(string Token);
        Task<bool> ValidateRefreshToken(string RefreshToken);
        Task<string> GetUserIdByRefreshToken(string RefreshToken);

        Task<int> AddRefreshToken(string userId, string refreshToken);
        Task<int> UpdateRefreshToken(string userid,string refreshToken);
        string GenerateToken(List<Claim> claims);
    }
}
