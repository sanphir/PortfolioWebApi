using System.ComponentModel.DataAnnotations;

namespace Portfolio.DAL.Models
{
    public class WorkTask : BaseEntity
    {

        [MaxLength(100)]
        public string Title { get; set; }


        [MaxLength(500)]
        public string Content { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public WorkTaskStatus Status { get; set; }

        public Employee Owner { get; set; }

        public Employee AssignedTo { get; set; }
    }
}
