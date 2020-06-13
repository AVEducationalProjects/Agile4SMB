using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Agile4SMB.Shared
{
    public class OrganizationUnitDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<BacklogDefinitionDTO> Backlogs { get; set; }
        public IEnumerable<OrganizationUnitDTO> Children { get; set; }

        public OrganizationUnitDTO()
        {
            Backlogs = new BacklogDefinitionDTO[] { };
            Children = new OrganizationUnitDTO[] { };
        }

        /// <summary>
        /// Gets backlog available to Organization Unit
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(BacklogDefinitionDTO backlog, OrganizationUnitDTO unit)> GetAvailableBacklogs()
        {
            var result = Backlogs
                .Select(x => (backlog: x, unit: this)).ToList();

            var childrenBacklogs = Children.SelectMany(x=>x.GetAvailableBacklogs());

            result.AddRange(childrenBacklogs);

            return result;
        }
    }
}
