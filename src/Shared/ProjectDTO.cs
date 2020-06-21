using System;

namespace Agile4SMB.Shared
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid UnitId { get; set; }

        public ProjectState State { get; set; }

    }
}