using System;
using System.Collections.Generic;

namespace Agile4SMB.Shared
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid UnitId { get; set; }

        public ProjectState State { get; set; }

        public IEnumerable<TaskDTO> Tasks { get; set; }

        public string StringState => State switch
        {
            ProjectState.New => "Новый",
            ProjectState.Approved => "Согласован",
            ProjectState.InProgress => "В работе",
            ProjectState.Done => "Завершен",
            ProjectState.Stopped => "Приостановлен",
            ProjectState.Canceled => "Отменен",
            _ => "не известно"
        };

        public ProjectDTO()
        {
            Tasks = new TaskDTO[] { };
        }
    }
}