using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.DAL.Models;
using Portfolio.WebApi.DTO;
using Portfolio.WebApi.Helpers;
using Portfolio.WebApi.Mappers;

namespace Portfolio.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTaskController : ControllerBase
    {
        private readonly DemoAppDbContext _context;
        private readonly IWorkTaskMapper _workTaskMapper;
        private readonly IValidator<WorkTask> _validator;
        private readonly IConfiguration _configuration;
        public WorkTaskController(DemoAppDbContext context, IValidator<WorkTask> validator, IWorkTaskMapper workTaskMapper, IConfiguration configuration)
        {
            _context = context;
            _workTaskMapper = workTaskMapper;
            _validator = validator;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> Get()
        {
            var result = (await _context.WorkTasks.ToListAsync()).Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> Get(Guid id)
        {
            var result = await _context.WorkTasks.FindAsync(id);
            if (result == null)
            {
                return BadRequest($"WorkTask id={id} not found");
            }

            return Ok(_workTaskMapper.MapWorkTaskDTO(result));
        }

        [Authorize]
        [HttpGet("allFor/{id}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> GetAllFor(Guid id)
        {
            var result = (await _context.WorkTasks
                .Include(task => task.Owner)
                .Include(task => task.AssignedTo)
                .Where(r => (r.AssignedTo != null && r.AssignedTo.Id == id)
                            || (r.Owner != null && r.Owner.Id == id))
                        .ToListAsync())
                        .Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpGet("assigndeTo/{id}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> GetAssignedTo(Guid id)
        {
            var result = (await _context.WorkTasks
                .Include(task => task.Owner)
                .Include(task => task.AssignedTo)
                .Where(r => r.AssignedTo != null && r.AssignedTo.Id == id)
                    .ToListAsync())
                    .Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpGet("createdBy/{id}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> GetCreatedBy(Guid id)
        {
            var result = (await _context.WorkTasks
                .Include(task => task.Owner)
                .Include(task => task.AssignedTo)
                .Where(r => r.Owner != null && r.Owner.Id == id)
                    .ToListAsync())
                    .Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult<WorkTaskDTO>> Add(NewWorkTaskDTO dto)
        {
            if (int.TryParse(_configuration.GetSection("Options:WorkTasksLimit")?.Value ?? "100", out int workTasksLimit))
            {
                if (_context.Employees.Count() >= workTasksLimit)
                {
                    return BadRequest($"You have reached the limit over {workTasksLimit} work tasks");
                }
            }

            if (string.IsNullOrEmpty(dto.Owner) || !dto.Owner.IsGuid() || dto.Owner.IsEmptyGuid())
            {
                dto.Owner = Request.Cookies[CookiesKeys.EMPLOYEE_ID];
            }

            var workTask = _workTaskMapper.MapWorkTask(dto);

            switch (dto.Status)
            {
                case WorkTaskStatus.Started:
                    workTask.StartedAt = DateTime.Now;
                    break;
                case WorkTaskStatus.Completed:
                    workTask.CompletedAt = DateTime.Now;
                    break;
            };

            var validateResult = _validator.Validate(workTask);
            if (!validateResult.IsValid)
            {
                var messages = string.Join("; ", validateResult.Errors.Select(e => $"{e.PropertyName}: \"{e.ErrorMessage}\"").ToArray());
                return BadRequest($"Work task validation error: {messages}");
            }

            _context.WorkTasks.Add(workTask);
            await _context.SaveChangesAsync();
            return Ok(_workTaskMapper.MapWorkTaskDTO(workTask));

        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<WorkTaskDTO>> Update(UpdateWorkTaskDTO dto)
        {
            var workTask = _context.WorkTasks.Find(dto.Id);
            if (workTask == null)
            {
                return BadRequest($"WorkTask \"{dto.Title}\" with id={dto.Id} not found");
            }

            var prevStatus = workTask.Status;
            _workTaskMapper.MapToExists(from: dto, to: workTask);

            if (prevStatus != workTask.Status)
            {
                switch (dto.Status)
                {
                    case WorkTaskStatus.Started:
                        workTask.StartedAt = DateTime.Now;
                        break;
                    case WorkTaskStatus.Completed:
                        workTask.CompletedAt = DateTime.Now;
                        break;
                };
            }

            var validateResult = _validator.Validate(workTask);
            if (!validateResult.IsValid)
            {
                var messages = string.Join("; ", validateResult.Errors.Select(e => $"{e.PropertyName}: \"{e.ErrorMessage}\"").ToArray());
                return BadRequest($"Work task validation error: {messages}");
            }

            await _context.SaveChangesAsync();
            return Ok(_workTaskMapper.MapWorkTaskDTO(workTask));

        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Remove(IEnumerable<Guid> idsToRemove)
        {
            var workTasks = _context.WorkTasks.Where(r => idsToRemove.Contains(r.Id));
            if (workTasks != null)
            {
                _context.WorkTasks.RemoveRange(workTasks);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
