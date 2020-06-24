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
        private IMongoCollection<Project> _projectCollection;

        public BacklogMongoRepository(IOptions<MongoOptions> mongoOptions)
        {
            var client = new MongoClient(mongoOptions.Value.Server);
            var database = client.GetDatabase(mongoOptions.Value.Database);

            _organizationUnitCollection = database.GetCollection<OrganizationUnit>(mongoOptions.Value.OrganizationUnitCollection);
            _projectCollection = database.GetCollection<Project>(mongoOptions.Value.ProjectCollection);

        }

        public void CreateBacklog(OrganizationUnit unit, BacklogDefinition backlogDefinition)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var updatedUnit = root.Find(unit.Id);

            updatedUnit.Backlogs = updatedUnit.Backlogs.Union(new[] { backlogDefinition }).ToArray();
            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);
        }

        public Backlog GetBacklog(Guid id) => new Backlog
        {
            Id = id,
            Projects = _projectCollection.FindSync(x => x.BacklogId == id).ToListAsync().Result.ToArray()
        };

        public void DeleteBacklog(Guid id)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var updatedUnit = root.FindWithBacklog(id);
            updatedUnit.Backlogs = updatedUnit.Backlogs.Where(x => x.Id != id).ToArray();
            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);

            _projectCollection.DeleteMany(x => x.BacklogId == id);
        }

        public void UpdateBacklog(BacklogDefinition backlog)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var updatedUnit = root.FindWithBacklog(backlog.Id);
            var updateDefinition = updatedUnit.Backlogs.Single(x => x.Id == backlog.Id);
            updateDefinition.Name = backlog.Name;

            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);
        }

        public void CreateProject(Backlog backlog, Project project)
        {
            project.BacklogId = backlog.Id;
            _projectCollection.InsertOne(project);
        }

        public Project GetProject(Guid id) => _projectCollection.FindSync(x => x.Id == id).SingleOrDefault();
        

        public void UpdateProject(Project project)
        {
            _projectCollection.ReplaceOne(x => x.Id == project.Id, project);
        }
    }
}
