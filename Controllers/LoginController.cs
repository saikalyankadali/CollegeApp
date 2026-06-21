using CollegeApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public ActionResult Login(LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide username and password");
            }

            if (login.UserName == "Admin" && login.Password == "Admin123")
            {
                var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("JWTSecret"));
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        //Username
                        new Claim(ClaimTypes.Name, login.UserName),
                        //Role
                        new Claim(ClaimTypes.Role, "Admin")
                    }),
                    Expires = DateTime.Now.AddHours(1),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenGenerated = tokenHandler.WriteToken(token);
                LoginResponseDTO response = new LoginResponseDTO() { UserName = login.UserName };
                response.Token = tokenGenerated;
                return Ok(response);
            }
            else
            {
                return Ok("Invalid Credintials");
            }
        }
    }
}
