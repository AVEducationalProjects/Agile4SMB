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

        /// <summary>
        /// Find unit by username in subtree
        /// </summary>
        /// <param name="username">username</param>
        /// <returns></returns>
        public OrganizationUnit Find(string username)
        {
            if (UserName == username)
                return this;
            return Children
                .Select(childUnit => childUnit.Find(username))
                .FirstOrDefault(result => result != null);
        }

        /// <summary>
        /// Find parent for unit with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OrganizationUnit FindParent(Guid id)
        {
            if (Children.Any(x=>x.Id==id))
                return this;
            return Children
                .Select(childUnit => childUnit.FindParent(id))
                .FirstOrDefault(result => result != null);
        }

        /// <summary>
        /// Find unit with backlog identified by id in subtree
        /// </summary>
        /// <param name="username">username</param>
        /// <returns></returns>
        public OrganizationUnit FindWithBacklog(Guid id)
        {
            if (Backlogs.Any(x=>x.Id==id))
                return this;
            return Children
                .Select(childUnit => childUnit.FindWithBacklog(id))
                .FirstOrDefault(result => result != null);        
        }
    }
}
