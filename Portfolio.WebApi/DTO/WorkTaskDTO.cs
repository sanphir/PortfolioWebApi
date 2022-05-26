﻿using Portfolio.DAL.Models;

namespace Portfolio.WebApi.DTO
{
    public class NewWorkTaskDTO
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime PlanedCompletedAt { get; set; }

        public DateTime CompletedAt { get; set; }

        public WorkTaskStatus Status { get; set; }

        public string Owner { get; set; }

        public string AssignedTo { get; set; }
    }

    public class UpdateWorkTaskDTO : NewWorkTaskDTO
    {
        public Guid Id { get; set; }
    }

    public class WorkTaskDTO : UpdateWorkTaskDTO
    {

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }
}