using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IUserService
    {
        User Authenticate(AuthenticateModel userObj);
        User Refresh(RefreshModel tokenObj);
        IEnumerable<User> GetAll();
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private static IList<User> _users = new List<User>
        { 
            new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" } 
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Generate a new jwt token, given a user. The token is stored in the user.
        /// </summary>
        /// <param name="user">User data object</param>
        private void GenerateTokens(ref User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.AuthenticateToken = tokenHandler.WriteToken(token);
            user.RefreshToken = GenerateRefreshToken();
        }

        /// <summary>
        /// Get the Claims principal from the expired token. This is so we can verify the user
        /// who wants to reset the token.
        /// src: https://www.blinkingcaret.com/2018/05/30/refresh-tokens-in-asp-net-core-web-api/
        /// </summary>
        /// <param name="token">Token to verify</param>
        /// <returns>claims principal, which contains the user id.</returns>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public User Authenticate(AuthenticateModel userObj)
        {
            var user = _users.SingleOrDefault(x => x.Username == userObj.Username && x.Password == userObj.Password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            GenerateTokens(ref user);
            return user.WithoutPassword();
        }
        
        public User Refresh(RefreshModel tokenObj)
        {
            var principal = GetPrincipalFromExpiredToken(tokenObj.AuthenticateToken);
            Int32 id;
            var result = System.Int32.TryParse(principal.Identity.Name, out id);
            if (!result)
                throw new SecurityTokenException("Incorrect authentication token");

            var user = _users.SingleOrDefault(x => x.Id == id);
            if (user == null)
                throw new SecurityTokenException("Token not recognised");

            if (tokenObj.RefreshToken != user.RefreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            GenerateTokens(ref user);
            return user.WithoutPassword();
        }

        public IEnumerable<User> GetAll()
        {
            return _users.WithoutPasswords();
        }
    }
}