using System;
using Agile4SMB.Shared.Domain;

namespace Agile4SMB.Server.Repositories
{
    public interface IBacklogRepository
    {
        void CreateBacklog(OrganizationUnit unit, BacklogDefinition backlogDefinition);
        Backlog Get(Guid id);
        void Delete(Guid id);
        void Update(BacklogDefinition backlog);
    }
}
