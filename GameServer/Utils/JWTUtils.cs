using GameServer.Models.Config;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameServer.Utils
{
    public class JWTUtils
    {
        public static readonly SymmetricSecurityKey SigningKey = new(Encoding.UTF8.GetBytes(ServerConfig.Instance.JWTSigningKey));
        private static readonly SigningCredentials Credentials = new(SigningKey, SecurityAlgorithms.HmacSha256); //TODO: maybe think of a better alternative?
        private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();
        public const string UserID = "UserID";
        public const string Role = "Role";
        public const string RoleModerator = "Moderator";
        public const string RoleUser = "User";
        public const string ModeratorPolicy = "Moderator";

        public static string GenerateToken(int userID, bool isModerator = false)
        {
            var claims = new[]
            {
                new Claim(UserID, userID.ToString()),
                new Claim(Role, isModerator ? RoleModerator : RoleUser)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: Credentials);

            return JwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
