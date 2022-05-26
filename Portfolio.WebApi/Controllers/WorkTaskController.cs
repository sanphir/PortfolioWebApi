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
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> Get()
        {
            var result = (await _context.WorkTasks.ToListAsync()).Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpGet("assigndeToUser/{id}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> GetAssignedToUserTasks(Guid id)
        {
            var result = (await _context.WorkTasks.Where(r => r.AssignedTo != null && r.AssignedTo.Id == id).ToListAsync()).Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpGet("createdByUser/{id}")]
        public async Task<ActionResult<IEnumerable<WorkTaskDTO>>> GetCreatedByUserTasks(Guid id)
        {
            var result = (await _context.WorkTasks.Where(r => r.Owner != null && r.Owner.Id == id).ToListAsync()).Select(r => _workTaskMapper.MapWorkTaskDTO(r));
            return Ok(result);
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult<WorkTaskDTO>> CreateWorkTask(NewWorkTaskDTO dto)
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
        public async Task<ActionResult<WorkTaskDTO>> UpdateTask(UpdateWorkTaskDTO dto)
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
        public async Task<ActionResult> RemoveWorkTask(IEnumerable<Guid> idsToRemove)
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
