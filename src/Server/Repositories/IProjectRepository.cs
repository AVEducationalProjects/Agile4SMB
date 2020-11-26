using System;
using System.Collections.Generic;
using Agile4SMB.Shared.Domain;

namespace Agile4SMB.Server.Repositories
{
    public interface IProjectRepository
    {
        Project Get(Guid id);
        IEnumerable<Project> GetByUnitId(Guid unitId);
    }
}