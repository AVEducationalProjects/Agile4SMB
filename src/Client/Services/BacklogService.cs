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
    public class BacklogService
    {
        private readonly HttpClient _http;

        public BacklogService(HttpClient http)
        {
            _http = http;
        }

        public async Task<Backlog> GetBacklog(BacklogDefinition backlogDefinition)
        {
            return await _http.GetFromJsonAsync<Backlog>($"api/Backlogs?id={backlogDefinition.Id}");
        }

        public async Task<(Backlog, Project)> CreateProjectInBacklog(string name, BacklogDefinition backlogDefinition, OrganizationUnit assignee)
        {
            var result = await _http.PostAsJsonAsync($"api/Projects",
                new CreateProjectDTO { Name = name, BacklogId = backlogDefinition.Id, AssigneeId = assignee.Id });

            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось добавить проект.");

            var createdProject = await result.Content.ReadFromJsonAsync<OrganizationUnit>();

            var backlog = await GetBacklog(backlogDefinition);
            var project = backlog.Projects.Single(x => x.Id == createdProject.Id);

            return (backlog, project);
        }

        public async Task UpdateProject(Project project)
        {
            var result = await _http.PatchAsJsonAsync($"api/Projects", project);

            if (!result.IsSuccessStatusCode)
                throw new ApplicationException("Не получилось обновить проект.");
        }


        public async Task CreateTaskInProject(Project project, string name, OrganizationUnit assignee)
        {
            project.Tasks = project.Tasks.Union(new[] {
                new ProjectTask {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Done = false,
                    UnitId = assignee.Id
                }}).ToArray();

            await UpdateProject(project);
        }

        public async Task DeleteTaskFromProject(Project project, ProjectTask projectTask)
        {
            project.Tasks = project.Tasks.Where(x => x.Id != projectTask.Id).ToArray();
            await UpdateProject(project);
        }

        public async Task AddProjectGoal(Project project, Guid id, string name)
        {
            project.Goals = project.Goals.Union(new[]
            {
                new ProjectGoal
                {
                    Name = name,
                    Id = id
                }
            }).ToArray();
            await UpdateProject(project);
        }

        public async Task DeleteProjectGoal(Project project, ProjectGoal goal)
        {
            project.Goals = project.Goals.Where(x => x.Id != goal.Id).ToArray();
            await UpdateProject(project);
        }

    }
}
