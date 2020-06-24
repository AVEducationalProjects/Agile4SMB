using System;
using System.Collections.Generic;
using Agile4SMB.Shared.Domain;

namespace Agile4SMB.Server.Repositories
{
    public interface IGoalRepository
    {
        IEnumerable<Goal> GetAll();
        void Create(Goal goal);
        Goal Get(Guid id);
        void Update(Goal goal);
        void Delete(Goal goal);
    }
}
