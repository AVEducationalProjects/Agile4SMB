using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agile4SMB.Client.Utils;
using Agile4SMB.Shared.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Agile4SMB.Client.Pages.Management
{
    public partial class ProjectManagement : ComponentBase, ISelectObserver<BacklogDefinition>, ISelectObserver<Project>
    {
        [Parameter] public Project SelectedProject { get; set; }
        [Parameter] public OrganizationUnit CurrentUser { get; set; }

        private BacklogDefinition SelectedBacklogDefinition { get; set; }
        protected Agile4SMB.Shared.Domain.Backlog SelectedBacklog { get; set; }
        private Project SelectedProjectFromSelectedBacklog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SelectedBacklogDefinition = CurrentUser.GetAvailableBacklogs().First().backlog;
            SelectedBacklog = await BacklogService.GetBacklog(SelectedBacklogDefinition);
            SelectedProjectFromSelectedBacklog = SelectedBacklog.Projects?.First();
            await base.OnInitializedAsync();
        }

        public BacklogDefinition Item => SelectedBacklogDefinition;

        public async Task Select(BacklogDefinition item)
        {
            SelectedBacklogDefinition = item;
            SelectedBacklog = await BacklogService.GetBacklog(SelectedBacklogDefinition);
            Update();
        }

        Project ISelectObserver<Project>.Item => SelectedProjectFromSelectedBacklog;

        public Task Select(Project item)
        {
            SelectedProjectFromSelectedBacklog = item;
            Update();
            return Task.CompletedTask;
        }

        public void Update()
        {
            StateHasChanged();
        }
        
        
        /* buttons reacts*/
        private async Task AddProjectToBacklog()
        {
            if (SelectedBacklogDefinition == null)
            {
                await JsRuntime.InvokeAsync<bool>("alert", "Не выбран беклог");
                return;
            }
            var copiedProject = await BacklogService
                .CreateProjectInBacklog(SelectedProject.Name, SelectedBacklogDefinition, CurrentUser);
            var project = await CopyProjectData(copiedProject.Item2);
            await BacklogService.UpdateProject(project);

            StateHasChanged();
        }

        private async Task AddTaskToProject(ProjectTask task)
        {
            if (SelectedProjectFromSelectedBacklog == null || SelectedBacklogDefinition == null)
            {
                await JsRuntime.InvokeAsync<bool>("alert", "Не выбран проект");
            }

            var response = await BacklogService.GetBacklog(SelectedBacklogDefinition);
            var project = response.Projects.Single(x => x.Id == SelectedProjectFromSelectedBacklog.Id);
            await BacklogService.CreateTaskInProject(project, task.Name, 
                UserUnitService.GetOrganizationUnit(task.UnitId)?? CurrentUser);
        }

        private async Task CreateProjectFromTask(ProjectTask task)
        {
            if (SelectedBacklogDefinition == null)
            {
                await JsRuntime.InvokeAsync<bool>("alert", "Не выбран беклог");
                return;
            }

            var response = await BacklogService
                .CreateProjectInBacklog(task.Name, SelectedBacklogDefinition, CurrentUser);
            response.Item2.UnitId = task.UnitId;
            await BacklogService.UpdateProject(response.Item2);
            
            await JsRuntime.InvokeAsync<bool>("alert", "Успешно");
            StateHasChanged();
        }

        protected async Task AddBacklog()
        {
            var copiedBacklog = await UserUnitService.CreateBacklog(CurrentUser, SelectedProject.Name);
            var projectInBacklog =
                await BacklogService.CreateProjectInBacklog(SelectedProject.Name, copiedBacklog, CurrentUser);
            var project = await CopyProjectData(projectInBacklog.Item2);

            await BacklogService.UpdateProject(project);
            StateHasChanged();

            await JsRuntime.InvokeAsync<bool>("alert", "Успешно");


        }

        private async Task<Project> CopyProjectData(Project project)
        {
            foreach (var task in SelectedProject.Tasks)
            {
                // skip tasks that we don't have access to
                if (UserUnitService.GetOrganizationUnit(task.UnitId) == null)
                    continue;
                await BacklogService.CreateTaskInProject(project, task.Name,
                    UserUnitService.GetOrganizationUnit(task.UnitId));
            }
                

            foreach (var goal in SelectedProject.Goals)
                await BacklogService.AddProjectGoal(project, goal.Id, goal.Name);

            var tasksToCopy = SelectedProject.Tasks.ToArray();
            for (var i = 0; i < project.Tasks.Count(); i++)
                project.Tasks.ToArray()[i].Done = tasksToCopy[i].Done;

            return project;
        }
        

    }
}