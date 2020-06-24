using Agile4SMB.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
            var updateInfo = new OrganizationUnit {Id = unit.Id, Name = unit.Name};
            var result = await  _http.PatchAsJsonAsync($"api/Organization", updateInfo);

            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось обновить подразделение.");
        }


        //====================

        private readonly List<Backlog> _backlogs = new List<Backlog>
        {
            new Backlog
            {
                Id = new Guid("00EFDF68-109A-42DB-BBF2-F2C914F3D3D5"),
                Projects = new List<Project>
                {
                    new Project
                    {
                        Id = new Guid("2D752FB1-D454-4574-80F4-4659B8DFE0C3"),
                        Name = "Оценка текущего бэклога ICL (не мобильные приложения)",
                        State = ProjectState.InProgress,
                        UnitId = new Guid("3ABBC46F-3243-4E45-855C-5AC702F5ECC8"),
                        Tasks = new List<ProjectTask>(),
                        Goals = new List<ProjectGoal>()
                    }
                }
            },
            new Backlog
            {
                Id = new Guid("3FCBF137-035A-4AB9-8288-16B6BDC7C1CC"),
                Projects = new List<Project>()
            }
        };


        public Backlog GetBacklog(BacklogDefinition backlogDefinition)
        {
            if (backlogDefinition == null)
                return new Backlog { Projects = new Project[] { } };

            return _backlogs.Single(x => x.Id == backlogDefinition.Id);
        }



        public async System.Threading.Tasks.Task CreateProjectInBacklog(BacklogDefinition backlogDefinition)
        {
            var backlog = GetBacklog(backlogDefinition);
            var unit = await GetCurrentUnit();
            ((List<Project>)backlog.Projects).Add(
                new Project
                {
                    Name = "Новый проект",
                    UnitId = unit.Id,
                    Tasks = new List<ProjectTask>(),
                    Goals = new List<ProjectGoal>()
                });
        }





       

        public void DeleteBacklog(BacklogDefinition backlogToDelete)
        {
            var unit = GetOwner(backlogToDelete, _currentUnit);
            ((IList<BacklogDefinition>)unit.Backlogs).Remove(backlogToDelete);

            static OrganizationUnit GetOwner(BacklogDefinition backlog, OrganizationUnit unit)
            {
                if (unit.Backlogs.Contains(backlog))
                    return unit;

                return unit
                    .Children
                    .FirstOrDefault(child => GetOwner(backlog, child) != null);
            }

        }


        public BacklogDefinition CreateBacklog(OrganizationUnit unit, string name)
        {
            var id = Guid.NewGuid();
            var newDef = new BacklogDefinition { Id = id, Name = name };
            var newBacklog = new Backlog { Id = id, Projects = new List<Project>() };

            _backlogs.Add(newBacklog);
            ((IList<BacklogDefinition>)unit.Backlogs).Add(newDef);

            return newDef;
        }

        private IList<Goal> _goals = new List<Goal>
        {
            new Goal{Id = new Guid("BB0A6A9D-0F83-461F-A5AE-CBF1FE0B781C"), Name = "Выручка с больших клиентов 20 млн. руб. (2020 год) "},
            new Goal{Id = new Guid("4363A829-82A5-412C-9995-4A9145E92606"), Name = "Выручка с существующих клиентов 15 млн. руб. (2020 год) "},
        };

        public IEnumerable<Goal> GetGoals()
        {
            return _goals;
        }

        public void CreateGoal(string name)
        {
            var id = Guid.NewGuid();
            _goals.Add(new Goal { Id = id, Name = name, Description = String.Empty });
        }

        public void DeleteGoal(Goal goal)
        {
            _goals.Remove(goal);
        }

        public async System.Threading.Tasks.Task CreateTaskInProject(Project project, string name)
        {
            var unit = await GetCurrentUnit();
            ((IList<ProjectTask>)project.Tasks).Add(new ProjectTask { Name = name, UnitId = unit.Id, Done = false });
        }

        public void DeleteTaskFromProject(Project project, ProjectTask projectTask)
        {
            if (project.Tasks.Contains(projectTask))
                ((IList<ProjectTask>)project.Tasks).Remove(projectTask);
        }

        public void DeleteProjectGoal(Project project, ProjectGoal goal)
        {
            if (project.Goals.Contains(goal))
                ((IList<ProjectGoal>)project.Goals).Remove(goal);
        }

        public void AddProjectGoal(Project project, Guid id, string name)
        {
            ((IList<ProjectGoal>)project.Goals).Add(new ProjectGoal { Id = id, Name = name });
        }
    }
}
