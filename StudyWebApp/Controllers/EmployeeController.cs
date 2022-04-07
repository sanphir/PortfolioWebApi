using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyProj.WebApp.DTO;
using StudyProj.WebApp.Mappers;

namespace StudyProj.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly StudyDbContext _context;
        private readonly IEmployeeMapper _employeeMapper;

        public EmployeeController(StudyDbContext context, IEmployeeMapper employeeMapper)
        {
            _context = context;
            _employeeMapper = employeeMapper;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> Get()
        {
            var result = (await _context.Employees.ToListAsync()).Select(e => _employeeMapper.MapEmployeeDTO(e));
            var t = new string[] { };
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDTO>> Get(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                return Ok(_employeeMapper.MapEmployeeDTO(employee));
            }
            else
            {
                return BadRequest($"Employee with id=\"{id}\"  not  found");
            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [HttpPost("add")]
        public async Task<ActionResult<EmployeeDTO>> AddEmployee(NewEmployeeDTO newEmployee)
        {
            var existingEmployee = _context.Employees.FirstOrDefault(r => r.Name == newEmployee.Name);
            if (existingEmployee == null)
            {
                var employee = _employeeMapper.MapNewEmployee(newEmployee);
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                return Ok(_employeeMapper.MapEmployeeDTO(employee));
            }
            else
            {
                return BadRequest($"Employee \"{newEmployee.Name}\" already exists");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [HttpDelete]
        public async Task<ActionResult> RemoveEmployee(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest($"Employee with id=\"{id}\"  not  found");
            }

        }
    }
}
