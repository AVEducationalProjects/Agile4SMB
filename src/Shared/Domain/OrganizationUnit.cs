using System;
using System.Collections.Generic;
using System.Linq;

namespace Agile4SMB.Shared.Domain
{
    public class OrganizationUnit
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }

        public IEnumerable<BacklogDefinition> Backlogs { get; set; }
        public IEnumerable<OrganizationUnit> Children { get; set; }

        public OrganizationUnit()
        {
            Backlogs = new List<BacklogDefinition>();
            Children = new List<OrganizationUnit>();
        }

        /// <summary>
        /// Gets backlog available to Organization Unit
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(BacklogDefinition backlog, OrganizationUnit unit)> GetAvailableBacklogs()
        {
            var result = Backlogs
                .Select(x => (backlog: x, unit: this)).ToList();

            var childrenBacklogs = Children.SelectMany(x=>x.GetAvailableBacklogs());

            result.AddRange(childrenBacklogs);

            return result;
        }

        /// <summary>
        /// Find unit by id in subtree
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public OrganizationUnit Find(Guid id)
        {
            if (Id == id)
                return this;
            return Children
                .Select(childUnit => childUnit.Find(id))
                .FirstOrDefault(result => result != null);
        }

        public OrganizationUnit Find(string username)
        {
            if (UserName == username)
                return this;
            return Children
                .Select(childUnit => childUnit.Find(username))
                .FirstOrDefault(result => result != null);
        }
    }
}
