using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public WorkTaskController(DemoAppDbContext context, IWorkTaskMapper workTaskMapper)
        {
            _context = context;
            _workTaskMapper = workTaskMapper;
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
            var result = (await _context.WorkTasks.Where(r => (r.AssignedTo != null && r.AssignedTo.Id == id)
                                                              || (r.Owner != null && r.Owner.Id == id))
                        .ToListAsync()).Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpGet("assigndeTo/{id}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> GetAssignedTo(Guid id)
        {
            var result = (await _context.WorkTasks.Where(r => r.AssignedTo != null && r.AssignedTo.Id == id).ToListAsync()).Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpGet("createdBy/{id}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> GetCreatedBy(Guid id)
        {
            var result = (await _context.WorkTasks.Where(r => r.Owner != null && r.Owner.Id == id).ToListAsync()).Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult<WorkTaskDTO>> Add(NewWorkTaskDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Owner) || !dto.Owner.IsGuid() || dto.Owner.IsEmptyGuid())
                {
                    dto.Owner = Request.Cookies[CookiesKeys.EMPLOYEE_ID];
                }

                var workTask = _workTaskMapper.MapWorkTask(dto);
                _context.WorkTasks.Add(workTask);
                await _context.SaveChangesAsync();
                return Ok(_workTaskMapper.MapWorkTaskDTO(workTask));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<WorkTaskDTO>> Update(UpdateWorkTaskDTO dto)
        {
            try
            {
                var workTask = _context.WorkTasks.Find(dto.Id);
                if (workTask == null)
                {
                    return BadRequest($"WorkTask \"{dto.Title}\" with id={dto.Id} not found");
                }

                _workTaskMapper.MapToExists(from: dto, to: workTask);
                await _context.SaveChangesAsync();
                return Ok(_workTaskMapper.MapWorkTaskDTO(workTask));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Remove(IEnumerable<Guid> idsToRemove)
        {
            try
            {
                var workTasks = _context.WorkTasks.Where(r => idsToRemove.Contains(r.Id));
                if (workTasks != null)
                {
                    _context.WorkTasks.RemoveRange(workTasks);
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
