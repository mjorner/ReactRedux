using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ReactRedux.Crypto;
using ReactRedux.Dtos;

namespace ReactRedux.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    public class AuthController : Controller {
        private readonly AppConfiguration Configuration;
        private readonly ICridentialsValidator CridentialsValidator;
        private readonly ILogger Logger;
        public AuthController(AppConfiguration configuration, ICridentialsValidator cridentialsValidator,
                              ILogger<AuthController> logger) {
            Configuration = configuration;
            CridentialsValidator = cridentialsValidator;
            Logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserDto userDto) {
            bool valid = CridentialsValidator.Verify(userDto.Username, userDto.Password);

            if (!valid) {
                Logger.LogInformation($"User \"{userDto.Username}\" not accepted.");
                return BadRequest("Username or password is incorrect");
            }

            Logger.LogInformation($"User \"{userDto.Username}\" accepted.");
            
            byte[] key = Encoding.ASCII.GetBytes(Configuration.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, userDto.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(Configuration.JWTExpireDayCount),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return Ok(new AuthResponseDto { Token = tokenString, Username = userDto.Username });
        }
    }
}