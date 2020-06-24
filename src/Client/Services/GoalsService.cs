using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Agile4SMB.Client.Utils;
using Agile4SMB.Shared.Domain;
using Agile4SMB.Shared.DTO;

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
            return await _http.GetFromJsonAsync<IEnumerable<Goal>>($"api/Goals");
        }

        public async Task CreateGoal(string name)
        {
            var result = await _http.PostAsJsonAsync($"api/Goals",
                new CreateGoalDTO{ Name = name, Description = String.Empty });
            
            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось добавить цель.");
        }

        public async Task DeleteGoal(Goal goal)
        {
            var result = await _http.DeleteAsync($"api/Goals?id={goal.Id}");
            
            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось удалить цель.");
        }

        public async Task UpdateGoal(Goal goal)
        {
            var result = await _http.PatchAsJsonAsync($"api/Goals", goal);
            
            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось обновить цель.");
        }
    }
}
