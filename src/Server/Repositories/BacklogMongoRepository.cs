using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agile4SMB.Server.Options;
using Agile4SMB.Shared.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Agile4SMB.Server.Repositories
{
    public class BacklogMongoRepository : IBacklogRepository
    {
        private IMongoCollection<OrganizationUnit> _organizationUnitCollection;
        private IMongoCollection<Backlog> _backlogCollection;

        public BacklogMongoRepository(IOptions<MongoOptions> mongoOptions)
        {
            var client = new MongoClient(mongoOptions.Value.Server);
            var database = client.GetDatabase(mongoOptions.Value.Database);

            _organizationUnitCollection = database.GetCollection<OrganizationUnit>(mongoOptions.Value.OrganizationUnitCollection);
            _backlogCollection = database.GetCollection<Backlog>(mongoOptions.Value.BacklogCollection);

        }

        public void CreateBacklog(OrganizationUnit unit, BacklogDefinition backlogDefinition)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var updatedUnit = root.Find(unit.Id);
         
            updatedUnit.Backlogs = updatedUnit.Backlogs.Union(new[] { backlogDefinition }).ToArray();

            _backlogCollection.InsertOne(new Backlog{Id=backlogDefinition.Id});
            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);
        }

        public Backlog Get(Guid id) => _backlogCollection.FindSync(x => x.Id == id ).SingleOrDefault();

        public void Delete(Guid id)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var updatedUnit = root.FindWithBacklog(id);
            updatedUnit.Backlogs = updatedUnit.Backlogs.Where(x =>x.Id != id).ToArray();
            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);

            _backlogCollection.DeleteOne(x => x.Id == id);
        }

        public void Update(BacklogDefinition backlog)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var updatedUnit = root.FindWithBacklog(backlog.Id);
            var updateDefinition = updatedUnit.Backlogs.Single(x =>x.Id == backlog.Id);
            updateDefinition.Name = backlog.Name;

            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);
        }
    }
}
