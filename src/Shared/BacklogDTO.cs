using System.Collections.Generic;

namespace Agile4SMB.Shared
{
    public class BacklogDTO
    {
        public long Id { get; set; }

        public IEnumerable<ProjectDTO> Projects { get; set; }

        public BacklogDTO()
        {
            Projects = new ProjectDTO[] { };
        }
    }
}
