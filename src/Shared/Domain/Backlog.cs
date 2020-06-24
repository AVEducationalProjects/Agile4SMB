using System;
using System.Collections.Generic;

namespace Agile4SMB.Shared.Domain
{
    public class Backlog
    {
        public Guid Id { get; set; }

        public IEnumerable<Project> Projects { get; set; }

        public Backlog()
        {
            Projects = new Project[] { };
        }
    }
}
