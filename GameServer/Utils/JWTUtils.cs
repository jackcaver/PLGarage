using GameServer.Models;
using GameServer.Models.Config;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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
        public static TimeSpan ExpirationTime => TimeSpan.FromHours(6);
        public static TimeSpan RefreshWindowStart => ExpirationTime.Subtract(TimeSpan.FromMinutes(120));
        public const string UserID = "UserID";
        public const string SessionID = "SessionID";
        public const string Role = "Role";
        public const string RoleModerator = "Moderator";
        public const string RoleUser = "User";
        public const string ModeratorPolicy = "Moderator";

        public static string GenerateToken(int userID, Guid sessionID, bool isModerator = false)
        {
            var token = JwtSecurityTokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor {
                Claims = new Dictionary<string, object>
                {
                    { UserID, userID.ToString() },
                    { SessionID, sessionID.ToString() },
                    { Role, isModerator ? RoleModerator : RoleUser }
                },
                IssuedAt = TimeUtils.Now,
                Expires = TimeUtils.Now.Add(ExpirationTime),
                SigningCredentials = Credentials
            });
            
            return JwtSecurityTokenHandler.WriteToken(token);
        }

        public static string GenerateToken(SessionInfo sessionInfo)
        {
            return GenerateToken(sessionInfo.UserId, sessionInfo.SessionId);
        }
        
        public static SessionInfo GetSessionInfo(ClaimsPrincipal user)
        {
            if (user == null)
                return new(0, Guid.Empty);
            
            var uidString = user.FindFirstValue(UserID);
            var sidString = user.FindFirstValue(SessionID);

            if (!string.IsNullOrEmpty(uidString)
                && !string.IsNullOrEmpty(sidString)
                && int.TryParse(uidString, out int userID)
                && Guid.TryParse(sidString, out Guid sessionID))
                return new(userID, sessionID);
            else
                return new(0, Guid.Empty);
        }
    }
}
