using System;
using System.Collections.Generic;
using System.Text;

namespace Agile4SMB.Shared.DTO
{
    public class CreateProjectDTO
    {
        public Guid BacklogId { get; set; }
        public string Name { get; set; }
        public Guid AssigneeId { get; set; }
    }
}
