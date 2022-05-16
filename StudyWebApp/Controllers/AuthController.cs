using Microsoft.AspNetCore.Mvc;
using StudyProj.DAL.Models;
using StudyProj.WebApp.Security;
using System.Security.Claims;

namespace StudyProj.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly StudyDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly bool _addDefaultAdmin;

        public AuthController(StudyDbContext context, IPasswordHasher passwordHasher, ITokenService tokenService, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _configuration = configuration;

            bool.TryParse(_configuration.GetSection("DB:AddDefaultAdmin")?.Value ?? "false", out _addDefaultAdmin);
            CheckDefaultAdmin();
        }

        private void CheckDefaultAdmin()
        {
            if (_addDefaultAdmin)
            {
                if (!_context.Employees.Any(x => x.Role == "admin"))
                {
                    var defaultAdmin = new Employee
                    {
                        Name = "admin",
                        Email = "admin@fake.mail.com",
                        Password = _passwordHasher.Hash("admin"),
                        Role = "admin",
                        BirthDate = DateTime.Now,
                        Salary = 1
                    };
                    _context.Employees.Add(defaultAdmin);
                    _context.SaveChanges();
                }
            }
        }

        [HttpPost("token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);

            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }
            var encodedJwt = _tokenService.GenerateToken(identity);

            //Response.Cookies.Append(key: "jwt", new CookiesOptions
            //{
            //    HttpOnly = true
            //});

            var response = new
            {
                accessToken = encodedJwt,
                refreshToken = "",
                //userName = identity.Claims.FirstOrDefault()?.Value ?? "",
                //role = identity.Claims.LastOrDefault()?.Value ?? "",
                //validTo = jwt.ValidTo
            };
            return Json(response);
        }

        [HttpPost("refreshToken")]
        public IActionResult RefreshToken(string accessToken, string refreshToken)
        {
            /*            var tokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = true,
                            ValidateIssuer = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = _authOptions.GetSymmetricSecurityKey(),
                            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
                        };*/
            return Ok();
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var employee = _context.Employees.FirstOrDefault(x => x.Name == username);

            if (employee != null && _passwordHasher.IsPasswordValid(employee.Password, password))
            {
                var claims = new List<Claim>
                {
                    new Claim("name", employee.Name),
                    new Claim("role", employee.Role)
                };

                return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            }

            return null;
        }
    }
}
