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
    public class UserUnitService
    {
        private readonly HttpClient _http;

        public UserUnitService(HttpClient http)
        {
            _http = http;
        }


        private OrganizationUnit _currentUnit;

        public async Task<OrganizationUnit> GetCurrentUnit()
        {
            if (_currentUnit == null)
                await LoadCurrentUnit();

            return _currentUnit;
        }

        private async Task LoadCurrentUnit()
        {
            _currentUnit = await _http.GetFromJsonAsync<OrganizationUnit>("api/Organization");
        }

        public IEnumerable<OrganizationUnit> GetOrganizationUnits()
        {
            return FlatternOrganizationUnits(_currentUnit);

            List<OrganizationUnit> FlatternOrganizationUnits(OrganizationUnit unit)
            {
                var result = new List<OrganizationUnit> { unit };

                foreach (var child in unit.Children)
                    result.AddRange(FlatternOrganizationUnits(child));

                return result;
            }
        }

        public OrganizationUnit GetOrganizationUnit(in Guid unitId)
        {
            return _currentUnit.Find(unitId);
        }

        public async  Task<IEnumerable<Project>> GetUnitProjects(Guid unitId)
        {
            return await _http
                .GetFromJsonAsync<IEnumerable<Project>>($"api/Projects?unitId={unitId}");
        }
        
        public async Task<OrganizationUnit> AddChildOrganizationUnit(OrganizationUnit unit, string name)
        {
            var result = await _http.PostAsJsonAsync("api/Organization",
                new CreateOrganizationUnitDTO { ParentId = unit.Id, Name = name });
            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось добавить подразделение.");

            var createdUnit = await result.Content.ReadFromJsonAsync<OrganizationUnit>();

            await LoadCurrentUnit();

            return GetOrganizationUnit(createdUnit.Id);
        }

        public async Task<OrganizationUnit> DeleteOrganizationUnit(OrganizationUnit unit)
        {
            if (_currentUnit == unit)
                return _currentUnit;

            var parent = _currentUnit.FindParent(unit.Id);

            var result = await _http.DeleteAsync($"api/Organization?id={unit.Id}");

            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось удалить подразделение.");

            await LoadCurrentUnit();

            return GetOrganizationUnit(parent.Id);
        }

        public async Task UpdateOrganizationUnit(OrganizationUnit unit)
        {
            var updateInfo = new OrganizationUnit { Id = unit.Id, Name = unit.Name };
            var result = await _http.PatchAsJsonAsync($"api/Organization", updateInfo);

            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось обновить подразделение.");
        }

        public async Task<BacklogDefinition> CreateBacklog(OrganizationUnit unit, string name)
        {
            var result = await _http.PostAsJsonAsync($"api/Backlogs",
                new CreateBacklogDTO { UnitId = unit.Id, Name = name });

            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось создать беклог.");

            var createdBacklogDef = await result.Content.ReadFromJsonAsync<BacklogDefinition>();

            await LoadCurrentUnit();

            return GetOrganizationUnit(unit.Id).Backlogs.Single(x => x.Id == createdBacklogDef.Id);
        }

        public async Task DeleteBacklog(BacklogDefinition backlogToDelete)
        {
            var result = await _http.DeleteAsync($"api/Backlogs?id={backlogToDelete.Id}");

            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось удалить беклог.");

            await LoadCurrentUnit();
        }

        public async Task UpdateBacklog(BacklogDefinition backlog)
        {
            var result = await _http.PatchAsJsonAsync($"api/Backlogs", backlog);

            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось обновить беклог.");
        }

        public async Task SetAccount(OrganizationUnit unit, string password)
        {
            var result = await _http.PostAsJsonAsync($"api/Accounts",
                new SetAccountDTO { UnitId = unit.Id, UserName = unit.UserName, Password = password});

            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось удалить изменить аккаунт.");

        }
    }
}
