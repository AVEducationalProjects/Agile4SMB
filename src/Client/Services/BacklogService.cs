using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Agile4SMB.Shared.Domain;

namespace Agile4SMB.Client.Services
{
    public class BacklogService
    {
        private readonly HttpClient _http;

        public BacklogService(HttpClient http)
        {
            _http = http;
        }
        
        public async Task<Backlog> GetBacklog(BacklogDefinition backlogDefinition)
        {
            //if (backlogDefinition == null)
            //    return new Backlog { Projects = new Project[] { } };

            //return _backlogs.Single(x => x.Id == backlogDefinition.Id);

            return new Backlog();
        }



        public async Task CreateProjectInBacklog(BacklogDefinition backlogDefinition)
        {
            //var backlog = GetBacklog(backlogDefinition);
            //var unit = await GetCurrentUnit();
            //((List<Project>)backlog.Projects).Add(
            //    new Project
            //    {
            //        Name = "Новый проект",
            //        UnitId = unit.Id,
            //        Tasks = new List<ProjectTask>(),
            //        Goals = new List<ProjectGoal>()
            //    });
        }


        public async Task CreateTaskInProject(Project project, string name)
        {
            //var unit = await GetCurrentUnit();
            //((IList<ProjectTask>)project.Tasks).Add(new ProjectTask { Name = name, UnitId = unit.Id, Done = false });
        }

        public async Task DeleteTaskFromProject(Project project, ProjectTask projectTask)
        {
            //if (project.Tasks.Contains(projectTask))
            //    ((IList<ProjectTask>)project.Tasks).Remove(projectTask);
        }

        public async Task AddProjectGoal(Project project, Guid id, string name)
        {
            //((IList<ProjectGoal>)project.Goals).Add(new ProjectGoal { Id = id, Name = name });
        }

        public async Task DeleteProjectGoal(Project project, ProjectGoal goal)
        {
            //if (project.Goals.Contains(goal))
            //    ((IList<ProjectGoal>)project.Goals).Remove(goal);
        }

    }
}
