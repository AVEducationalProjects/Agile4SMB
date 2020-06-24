using System;
using System.Collections.Generic;

namespace Agile4SMB.Shared.Domain
{
    public class Project
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid UnitId { get; set; }

        public Guid BacklogId { get; set; }

        public ProjectState State { get; set; }

        public IEnumerable<ProjectTask> Tasks { get; set; }

        public IEnumerable<ProjectGoal> Goals { get; set; }

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


        public Project()
        {
            Tasks = new ProjectTask[] { };
            Goals = new ProjectGoal[] { };
        }
    }
}