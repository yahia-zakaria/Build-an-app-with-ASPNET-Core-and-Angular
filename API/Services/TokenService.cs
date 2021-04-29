using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService(ITokenService tokenService, IConfiguration config)
        {
            _config = config;
            _tokenService = tokenService;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["tokenKey"]));
        }


        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>(){
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var descriptor = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            
            return tokenHandler.WriteToken(token);
        }
    }
}