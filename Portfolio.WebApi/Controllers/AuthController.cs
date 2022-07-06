using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Portfolio.DAL.Models;
using Portfolio.WebApi.Security;

namespace Portfolio.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly DemoAppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly bool _addDefaultAdmin;
        private readonly CookieOptions _cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        };

        public AuthController(DemoAppDbContext context, IPasswordHasher passwordHasher, ITokenService tokenService, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;

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

            _logger.LogInformation("{datetime:yyyy-MM-dd HH:mm:ss:fffff}: User \"{username}\" got new token", DateTimeOffset.UtcNow, username);

            return Json(new { accessToken, employeeId = authEmployee.Id });
        }

        [HttpPost("refreshToken")]
        public IActionResult RefreshToken(string accessToken)
        {
            if (string.IsNullOrEmpty(Request.Cookies[CookiesKeys.EMPLOYEE_ID]) || string.IsNullOrEmpty(Request.Cookies[CookiesKeys.REFRESH_TOKEN]))
            {
                return Unauthorized();
            }

            var refreshToken = Request.Cookies[CookiesKeys.REFRESH_TOKEN];
            var employeeId = Guid.Parse(Request.Cookies[CookiesKeys.EMPLOYEE_ID]);
            var employee = _context.Employees.Find(employeeId);

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

                return Json(new { accessToken = newAccessToken, employeeId });
            }
            catch (SecurityTokenException stex)
            {
                return Unauthorized(stex.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("signout")]
        public IActionResult Signout()
        {
            var username = Request.Cookies[CookiesKeys.EMPLOYEE_NAME];
            Response.Cookies.Delete(CookiesKeys.REFRESH_TOKEN, _cookieOptions);
            Response.Cookies.Delete(CookiesKeys.EMPLOYEE_NAME, _cookieOptions);
            Response.Cookies.Delete(CookiesKeys.EMPLOYEE_ID, _cookieOptions);

            _logger.LogInformation("{datetime:yyyy-MM-dd HH:mm:ss:fffff}: User \"{username}\" was sign out", DateTimeOffset.UtcNow, username);
            return Ok();
        }

        private string ProduceAccessToken(Employee emplyee)
        {
            var accessToken = _tokenService.GenerateToken(emplyee);
            var refreshToken = _tokenService.GenerateRefreshToken(emplyee);

            Response.Cookies.Append(CookiesKeys.REFRESH_TOKEN, refreshToken, _cookieOptions);
            Response.Cookies.Append(CookiesKeys.EMPLOYEE_NAME, emplyee.Name, _cookieOptions);
            Response.Cookies.Append(CookiesKeys.EMPLOYEE_ID, emplyee.Id.ToString(), _cookieOptions);

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
                if (!_context.Employees.Any(x => x.Role == Role.Admin))
                {
                    var defaultAdmin = new Employee
                    {
                        Name = "admin",
                        Email = "admin@fake.mail.com",
                        Password = _passwordHasher.Hash("admin"),
                        Role = Role.Admin,
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
