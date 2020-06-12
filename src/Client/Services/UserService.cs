using Agile4SMB.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agile4SMB.Client.Services
{
    public class UserService
    {

        /// <summary>
        /// Get backlogs for user
        /// </summary>
        /// <returns>List of backlog metadata</returns>
        public IEnumerable<BacklogDTO> GetBacklogs()
        {
            return new[] { new BacklogDTO { Name = "Беклог 1", OrganizationUnit = new OrganizationUnitDTO { Id = 1, Name = "Коммерческий департамент" } } };
        }
    }
}
