using Portfolio.DAL.Models;
using Portfolio.WebApi.DTO;

namespace Portfolio.WebApi.Mappers
{
    public class WorkTaskMapper : IWorkTaskMapper
    {
        private readonly DemoAppDbContext _context;
        public WorkTaskMapper(DemoAppDbContext context)
        {
            _context = context;
        }
        public WorkTaskDTO MapWorkTaskDTO(WorkTask workTask)
        {
            return new WorkTaskDTO
            {
                Id = workTask.Id,
                Title = workTask.Title,
                Content = workTask.Content,
                PlanedCompletedAt = workTask.CompletedAt,
                CompletedAt = workTask.CompletedAt,
                Status = workTask.Status,
                Owner = workTask.Owner?.Id.ToString() ?? "",
                AssignedTo = workTask.AssignedTo?.Id.ToString() ?? "",
                CreatedDate = workTask.CreatedDate,
                LastModifiedDate = workTask.LastModifiedDate
            };
        }

        public WorkTask MapWorkTask(NewWorkTaskDTO dto)
        {
            var assignedEmployee = _context.Employees.Find(dto.AssignedTo);
            var ownerEmployee = _context.Employees.Find(dto.Owner);
            return new WorkTask
            {
                Title = dto.Title,
                Content = dto.Content,
                PlanedCompletedAt = dto.CompletedAt,
                CompletedAt = dto.CompletedAt,
                Status = dto.Status,
                Owner = ownerEmployee,
                AssignedTo = assignedEmployee
            };
        }

        public void MapToExists(UpdateWorkTaskDTO from, WorkTask to)
        {
            to.Title = from.Title;
            to.Content = from.Content;
            to.PlanedCompletedAt = from.CompletedAt;
            to.CompletedAt = from.CompletedAt;
            to.Status = from.Status;

            var assignedEmployee = _context.Employees.Find(to.AssignedTo);
            var ownerEmployee = _context.Employees.Find(to.Owner);

            to.AssignedTo = assignedEmployee;
            to.Owner = ownerEmployee;
        }
    }
}
