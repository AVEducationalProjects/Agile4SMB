using System;

namespace Agile4SMB.Shared
{
    public class TaskDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid UnitId { get; set; }
        public bool Done { get; set; }
    }
}