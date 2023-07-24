using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpleJWT.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration configuration;
        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Auth([FromBody] User user)
        {
            IActionResult response = Unauthorized();
            UserModel urs1 = Authenticate(user);
            if (urs1 != null)
            {
                //if (user.UserName.Equals("adeeb") && user.Password.Equals("adeeb"))
                //{
                    var issuer = configuration["Jwt:Issuer"];
                    var audience = configuration["Jwt:Audience"];
                    var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
                    var signingCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature
                    );

                    var subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, urs1.Username),
                        new Claim(JwtRegisteredClaimNames.Email, urs1.Username),
                        new Claim(ClaimTypes.Role,urs1.Role)
                    });

                    var expires = DateTime.UtcNow.AddMinutes(10);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = subject,
                        Expires = expires,
                        Issuer = issuer,
                        Audience = audience,
                        SigningCredentials = signingCredentials
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);

                    return Ok(jwtToken);
                //}
            }

            return response;
        }

        


        /*
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] User userLogin)
        {
            //var user = Authenticate(userLogin);
            //if (user != null)
            //{
            //    var token = GenerateToken(user);
            //    return Ok(token);
            //}

            return Ok(userLogin);
        }
        */

        //To authenticate user
        private UserModel Authenticate(User userLogin)
        {
            var currentUser = UserConstants.Users.FirstOrDefault(x => x.Username.ToLower() ==
                userLogin.UserName.ToLower() && x.Password == userLogin.Password);
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
    }
}
