using System;
using Agile4SMB.Server.Model;
using Agile4SMB.Shared;
using Agile4SMB.Shared.Domain;

namespace Agile4SMB.Server.Repositories
{
    public interface IOrganizationUnitRepository
    {
        OrganizationUnit Get(string username);
        OrganizationUnit Get(Guid id);
        OrganizationUnit GetOwner(Guid backlogId);
        void AddToParent(OrganizationUnit parent, OrganizationUnit child);
        void Delete(OrganizationUnit unit);
        void Update(OrganizationUnit unit);
        void SetAccount(OrganizationUnit unit, Account account);
        Account GetAccount(string username);
    }
}
