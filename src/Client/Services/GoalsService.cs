using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Agile4SMB.Shared.Domain;

namespace Agile4SMB.Client.Services
{
    public class GoalsService
    {
        private readonly HttpClient _http;

        public GoalsService(HttpClient http)
        {
            _http = http;
        }
        
        public async Task<IEnumerable<Goal>> GetGoals()
        {
            return new Goal[] { };
        }

        public async Task CreateGoal(string name)
        {
            //var id = Guid.NewGuid();
            //_goals.Add(new Goal { Id = id, Name = name, Description = String.Empty });
        }

        public async Task DeleteGoal(Goal goal)
        {
            //_goals.Remove(goal);
        }
    }
}
