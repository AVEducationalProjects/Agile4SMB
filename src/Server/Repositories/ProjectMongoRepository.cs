using System;
using System.Collections.Generic;
using System.Diagnostics;
using Agile4SMB.Server.Options;
using Agile4SMB.Shared.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Agile4SMB.Server.Repositories
{
    public class ProjectMongoRepository : IProjectRepository
    {
        private readonly IMongoCollection<Project> _projectCollection;
        
        public ProjectMongoRepository(IOptions<MongoOptions> mongoOptions)
        {
            var client = new MongoClient(mongoOptions.Value.Server);
            var database = client.GetDatabase(mongoOptions.Value.Database);

            _projectCollection = database
                .GetCollection<Project>(mongoOptions.Value.ProjectCollection);
        }
        
        
        public Project Get(Guid id)
        {
            return _projectCollection
                .FindSync(x => x.Id == id).Single();
        }

        public IEnumerable<Project> GetByUnitId(Guid unitId)
        {
            var data = _projectCollection.FindSync(x => x.UnitId == unitId).ToEnumerable<Project>();
            return data;  
        }
    }
}