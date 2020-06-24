using System;
using Agile4SMB.Shared.Domain;

namespace Agile4SMB.Server.Repositories
{
    public interface IBacklogRepository
    {
        void CreateBacklog(OrganizationUnit unit, BacklogDefinition backlogDefinition);
        Backlog GetBacklog(Guid id);
        void DeleteBacklog(Guid id);
        void UpdateBacklog(BacklogDefinition backlog);
        void CreateProject(Backlog backlog, Project project);
        Project GetProject(Guid id);
        void UpdateProject(Project project);
    }
}
