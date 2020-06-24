using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Agile4SMB.Server.Model;
using Agile4SMB.Server.Options;
using Agile4SMB.Server.Repositories;
using Agile4SMB.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Agile4SMB.Server.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IOrganizationUnitRepository _organizationUnitRepository;
        private readonly JWTOptions _jwtOptions;

        public LoginController(IOrganizationUnitRepository organizationUnitRepository, IOptions<JWTOptions> jwtOptions)
        {
            _organizationUnitRepository = organizationUnitRepository;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost]
        [Route("/api/token")]
        [AllowAnonymous]
        public ActionResult<TokenDTO> GetToken(LoginDTO loginData)
        {
            var account = _organizationUnitRepository.GetAccount(loginData.UserName);

            if (account == null)
                return NotFound();

            if (account.PasswordHash != AccountsController.Hash(loginData.Password))
                return Unauthorized();

            var result = new TokenDTO
            {
                AccessToken = CreateJWT(account)
            };

            return Created("", result);
        }

        private string CreateJWT(Account account)
        {
            var subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, account.UserName),
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "Agile4SMB",
                Audience = "*",
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.UtcNow.AddDays(7),
                Subject = subject,
                SigningCredentials = new SigningCredentials(_jwtOptions.GetSigningKey(), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
