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
    public class GoalMongoRepository : IGoalRepository
    {
        private readonly IMongoCollection<Goal> _goalCollection;

        public GoalMongoRepository(IOptions<MongoOptions> mongoOptions)
        {
            var client = new MongoClient(mongoOptions.Value.Server);
            var database = client.GetDatabase(mongoOptions.Value.Database);

            _goalCollection = database.GetCollection<Goal>(mongoOptions.Value.GoalCollection);
        }

        public IEnumerable<Goal> GetAll() => _goalCollection.FindSync(_ => true).ToList().ToArray();

        public void Create(Goal goal)
        {
            _goalCollection.InsertOne(goal);
        }

        public Goal Get(Guid id) => _goalCollection.FindSync(x => x.Id == id).SingleOrDefault();

        public void Update(Goal goal)
        {
            _goalCollection.FindOneAndReplace(x => x.Id == goal.Id, goal);
        }

        public void Delete(Goal goal)
        {
            _goalCollection.FindOneAndDelete(x => x.Id == goal.Id);
        }
    }
}
