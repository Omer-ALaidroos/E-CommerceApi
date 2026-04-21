using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace eCommerceApp.Infrastructure.Repository.Authentication
{
    public class TokenManagement(AppDbContext context, IConfiguration config) : ITokenManagements
    {
        public async Task<int> AddRefreshToken(string userId, string refreshToken)
        {
            context.RefreshTokens.Add(
                 new RefreshToken
                 {

                     UserId = userId,
                     Token = refreshToken
                 } 
             );
            
            return await context.SaveChangesAsync();
        }

        public string GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(2);
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetRefreshToken()
        {
            const int baytSize = 64;
            byte[] randomeByte = new byte[baytSize];
            using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
            {
                rnd.GetBytes(randomeByte);
            }
            // Use a URL-safe variant of Base64 encoding to avoid issues with special characters.
            string token = Convert.ToBase64String(randomeByte);
            return token.Replace('+', '-').Replace('/', '_');
        }

        public List<Claim> GetUserClaimsFromToken(string Token)
        {
           var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(Token);

            if (jwtToken != null)
                return jwtToken.Claims.ToList();
            else
                return [];
        }

        public async Task<string> GetUserIdByRefreshToken(string RefreshToken)
        {
            return (await context.RefreshTokens.FirstOrDefaultAsync(_ => _.Token == RefreshToken))!.UserId;
        }

        public async Task<int> UpdateRefreshToken(string userid, string refreshToken)
        {
         
            RefreshToken? userToken = await context.RefreshTokens.FirstOrDefaultAsync(_ => _.UserId == userid);// لا يوجد توكن لهذا المستخدم لتحديثه
            if (userToken != null)
            {
                userToken.Token = refreshToken;
                return await context.SaveChangesAsync();
            }
            // If no token is found, you might want to add one or log an error.
            // For now, we'll return 0 to indicate no rows were affected.
            return 0;
        }

        public async Task<bool> ValidateRefreshToken(string RefreshToken)
        {
            var user = await context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(_ => _.Token == RefreshToken);
            return (user != null);
        }
    }
}
