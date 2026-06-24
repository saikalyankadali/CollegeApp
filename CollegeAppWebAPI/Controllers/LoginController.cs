using CollegeApp.Models;
using CollegeApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public LoginController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost]
        public async Task<APIResponse> Login(LoginDTO login)
        {
            var apiResponse = new APIResponse();

            if (!ModelState.IsValid)
            {
                apiResponse.Status = false;
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.Errors.Add("Please provide username and password");
                return apiResponse;
            }

            var user = await _userService.GetUserForLoginAsync(login.UserName);

            // PBKDF2 verification using the stored hash + salt — NOT BCrypt
            bool isValid =
                user != null
                && user.IsActive
                && !user.IsDeleted
                && _userService.VerifyPassword(login.Password, user.Password, user.PasswordSalt);

            if (!isValid)
            {
                apiResponse.Status = false;
                apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                apiResponse.Errors.Add("Invalid Credentials");
                return apiResponse;
            }

            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("JWTSecret"));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.RoleName)
                }),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenGenerated = tokenHandler.WriteToken(token);

            LoginResponseDTO response = new LoginResponseDTO
            {
                UserName = user.Username,
                Token = tokenGenerated
            };

            apiResponse.Status = true;
            apiResponse.StatusCode = HttpStatusCode.OK;
            apiResponse.Data = response;
            return apiResponse;
        }
    }
}