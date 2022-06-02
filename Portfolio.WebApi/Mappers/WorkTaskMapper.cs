using Portfolio.DAL.Models;
using Portfolio.WebApi.DTO;
using Portfolio.WebApi.Helpers;

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
                DueDate = workTask.DueDate,
                StartedAt = workTask.StartedAt,
                CompletedAt = workTask.CompletedAt,
                Status = workTask.Status,
                Owner = workTask.Owner?.Id.ToString() ?? "",
                OwnerName = workTask.Owner?.Name ?? "",
                AssignedTo = workTask.AssignedTo?.Id.ToString() ?? "",
                AssignedToName = workTask.AssignedTo?.Name ?? "",
                CreatedDate = workTask.CreatedDate,
                LastModifiedDate = workTask.LastModifiedDate
            };
        }

        public WorkTask MapWorkTask(NewWorkTaskDTO dto)
        {
            Employee assignedEmployee = null;
            if (dto.AssignedTo.IsGuid() && Guid.TryParse(dto.AssignedTo, out Guid assignedToId))
            {
                assignedEmployee = _context.Employees.Find(assignedToId);
            }

            Employee ownerEmployee = null;
            if (dto.Owner.IsGuid() && Guid.TryParse(dto.Owner, out Guid ownerId))
            {
                ownerEmployee = _context.Employees.Find(ownerId);
            }

            return new WorkTask
            {
                Title = dto.Title,
                Content = dto.Content,
                DueDate = dto.DueDate,
                Status = dto.Status,
                Owner = ownerEmployee,
                AssignedTo = assignedEmployee
            };
        }

        public void MapToExists(UpdateWorkTaskDTO fromDto, WorkTask toModel)
        {
            toModel.Title = fromDto.Title;
            toModel.Content = fromDto.Content;
            toModel.DueDate = fromDto.DueDate;
            toModel.Status = fromDto.Status;

            Employee assignedEmployee = null;
            if (fromDto.AssignedTo.IsGuid() && Guid.TryParse(fromDto.AssignedTo, out Guid assignedToId))
            {
                assignedEmployee = _context.Employees.Find(assignedToId);
            }

            Employee ownerEmployee = null;
            if (fromDto.Owner.IsGuid() && Guid.TryParse(fromDto.Owner, out Guid ownerId))
            {
                ownerEmployee = _context.Employees.Find(ownerId);
            }

            toModel.AssignedTo = assignedEmployee;
            toModel.Owner = ownerEmployee;
        }
    }
}
