using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TOKENDEMO.Models;
using TOKENDEMO.Repository;

namespace TOKENDEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private IConfiguration _configuration { get; }
        public AuthController(IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserAuth userAuth)
        {
            var user = (await _userRepository.GetAllAsync())
                .FirstOrDefault(u => u.Email == userAuth.Email
                && u.Password == userAuth.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            var tokenString = GenerateToken(user);
            user.RefreshToken = Guid.NewGuid().ToString();
            // Set expiry time for refresh token
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
            await _userRepository.UpdateAsync(user);
            return Ok(new { Token = tokenString, RefreshToken = user.RefreshToken });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            var user = (await _userRepository.GetAllAsync())
                .FirstOrDefault(u => u.RefreshToken == request.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized();
            }
            var tokenString = GenerateToken(user);
            user.RefreshToken = Guid.NewGuid().ToString();
            // Set expiry time for refresh token
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
            await _userRepository.UpdateAsync(user);
            return Ok(new { Token = tokenString, RefreshToken = user.RefreshToken });
        }
        private string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            //tạo đối tương JwtSecurityTokenHandler để tạo và ký token.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
            // tạo một đối tượng SecurityTokenDescriptor chứa thông tin về token như:
            //Subject: Danh tính của người dùng, bao gồm các claim như tên và vai trò.
            //Expires: Thời gian sống của token (30 phút).
            //SigningCredentials: Thông tin để ký token, bao gồm khóa bí mật và thuật toán ký.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public class TokenRequest
        {
            public string RefreshToken { get; set; }
        }
    }
}
