﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudyProj.DAL.Models;
using StudyProj.WebApp.Auth;
using StudyProj.WebApp.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudyProj.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly StudyDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthOptions _authOptions;
        private readonly IConfiguration _configuration;
        private readonly bool _addDefaultAdmin;

        public AuthController(StudyDbContext context, IPasswordHasher passwordHasher, IAuthOptions authOptions, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _authOptions = authOptions;
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

        [HttpPost("/token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);

            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;


            var jwt = new JwtSecurityToken(
                    issuer: _authOptions.Issuer,
                    audience: _authOptions.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(_authOptions.LifeTime)),
                    signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //Response.Cookies.Append(key: "jwt", new CookiesOptions
            //{
            //    HttpOnly = true
            //});

            var response = new
            {
                accessToken = encodedJwt,
                userName = identity.Claims.FirstOrDefault()?.Value ?? "",
                role = identity.Claims.LastOrDefault()?.Value ?? "",
                validTo = jwt.ValidTo
            };
            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var employee = _context.Employees.FirstOrDefault(x => x.Name == username);

            if (employee != null && _passwordHasher.IsPasswordValid(employee.Password, password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, employee.Name),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, employee.Role)
                };

                return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            }

            return null;
        }
    }
}
