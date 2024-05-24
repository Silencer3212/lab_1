using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using ProtectedWeb.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProtectedWeb.Utils
{
    public class JwtPayload
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
    }
    public class UserMethods
    {
        public static string GetPasswordHash(string password)
        {
            using (SHA256 SHA256 = SHA256.Create())
            {
                byte[] hashed_password_bytes = SHA256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToHexString(hashed_password_bytes);
            }
        }

        public static JwtPayload? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = AuthOptions.GetSymmetricSecurityKey();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                var roleId = int.Parse(jwtToken.Claims.First(x => x.Type == "role").Value);
                JwtPayload result = new JwtPayload
                {
                    Id = userId,
                    RoleId = roleId
                };
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static bool HaveModerRights(string token)
        {
            JwtPayload? payload = ValidateToken(token);
            if (payload == null)
            {
                return false;
            }
            int? roleId = payload.RoleId;
            if (roleId == 1 || roleId == 2) {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool HaveAdminRights(string token)
        {
            JwtPayload? payload = ValidateToken(token);
            if (payload == null)
            {
                return false;
            }
            int? roleId = payload.RoleId;
            if (roleId == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
