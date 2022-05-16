using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudyProj.DAL.Models;
using StudyProj.WebApp.Security;

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
        private const string COOKIES_KEY_REFRESH_TOKEN = "refreshToken";
        private const string COOKIES_KEY_NAME = "name";
        private readonly CookieOptions _cookieOptions = new()
        {
            HttpOnly = true
        };

        public AuthController(StudyDbContext context, IPasswordHasher passwordHasher, ITokenService tokenService, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _configuration = configuration;

            bool.TryParse(_configuration.GetSection("DB:AddDefaultAdmin")?.Value ?? "false", out _addDefaultAdmin);
            CheckDefaultAdmin();
        }

        [HttpPost("token")]
        public IActionResult Token(string username, string password)
        {
            var authEmployee = GetAuthEmployee(username, password);

            if (authEmployee == null)
            {
                return Unauthorized(new { errorText = "Invalid username or password." });
            }

            var accessToken = ProduceAccessToken(authEmployee);

            return Json(new { accessToken });
        }

        [HttpPost("refreshToken")]
        public IActionResult RefreshToken(string accessToken)
        {
            var refreshToken = Request.Cookies[COOKIES_KEY_REFRESH_TOKEN];
            var name = Request.Cookies[COOKIES_KEY_NAME];
            var employee = _context.Employees.FirstOrDefault(x => x.Name == name);

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken) || employee == null)
            {
                return Unauthorized();
            }

            try
            {
                var accessJwtsecurToken = _tokenService.GetJwtSecurityToken(accessToken, validateLifetime: false);
                var refreshJwtSecurToken = _tokenService.GetJwtSecurityToken(refreshToken, validateLifetime: true);

                if (!Equals(employee.Name, accessJwtsecurToken?.Payload["name"] ?? "") || !Equals(employee.Name, refreshJwtSecurToken?.Payload["name"] ?? ""))
                {
                    return Unauthorized("Invalid token");
                }
                var newAccessToken = ProduceAccessToken(employee);

                return Json(new { accessToken = newAccessToken });
            }
            catch (SecurityTokenException stex)
            {
                return Unauthorized(stex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private string ProduceAccessToken(Employee emplyee)
        {
            var accessToken = _tokenService.GenerateToken(emplyee);
            var refreshToken = _tokenService.GenerateRefreshToken(emplyee);

            Response.Cookies.Append(COOKIES_KEY_REFRESH_TOKEN, refreshToken, _cookieOptions);
            Response.Cookies.Append(COOKIES_KEY_NAME, emplyee.Name, _cookieOptions);

            return accessToken;
        }

        private Employee GetAuthEmployee(string name, string password)
        {
            var employee = _context.Employees.FirstOrDefault(x => x.Name == name);

            if (employee != null && _passwordHasher.IsPasswordValid(employee.Password, password))
            {
                return employee;
            }

            return null;
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
    }
}
