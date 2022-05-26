using Portfolio.DAL.Models;
using Portfolio.WebApi.DTO;

namespace Portfolio.WebApi.Mappers
{
    public interface IWorkTaskMapper
    {
        WorkTaskDTO MapWorkTaskDTO(WorkTask workTask);

        WorkTask MapWorkTask(NewWorkTaskDTO dto);

        void MapToExists(UpdateWorkTaskDTO from, WorkTask to);
    }
}
