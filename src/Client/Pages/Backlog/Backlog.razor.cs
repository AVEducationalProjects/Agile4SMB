using System.Linq;
using System.Threading.Tasks;
using Agile4SMB.Client.Services;
using Agile4SMB.Client.Utils;
using Agile4SMB.Shared;
using Agile4SMB.Shared.Domain;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;


namespace Agile4SMB.Client.Pages.Backlog
{
    public class BacklogBase : ComponentBase, ISelectObserver<BacklogDefinition>, ISelectObserver<Project>
    {
        [Inject] public UserUnitService UserUnitService { get; set; }
        [Inject] public BacklogService BacklogService { get; set; }

        protected OrganizationUnit CurrentUnit { get; set; }
        protected BacklogDefinition CurrentBacklogDefinition { get; set; }
        protected Agile4SMB.Shared.Domain.Backlog CurrentBacklog { get; set; }
        protected Project CurrentProject { get; set; }

        protected override async Task OnInitializedAsync()
        {
            CurrentProject = null;
            CurrentUnit = await UserUnitService.GetCurrentUnit();
            CurrentBacklogDefinition = CurrentUnit.GetAvailableBacklogs().First().backlog;
            CurrentBacklog = await BacklogService.GetBacklog(CurrentBacklogDefinition);
            Update();
        }

        BacklogDefinition ISelectObserver<BacklogDefinition>.Item => CurrentBacklogDefinition;

        public async Task Select(BacklogDefinition item)
        {
            CurrentProject = null;
            CurrentBacklogDefinition = item;
            CurrentBacklog = await BacklogService.GetBacklog(CurrentBacklogDefinition);
            Update();
        }

        public void Update()
        {
            StateHasChanged();
        }

        Project ISelectObserver<Project>.Item => CurrentProject;
        public Task Select(Project item)
        {
            CurrentProject = item;
            Update();
            return Task.CompletedTask;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (CurrentBacklogDefinition == null)
            {
                var unit = await UserUnitService.GetCurrentUnit();
                CurrentBacklogDefinition = unit.Backlogs.FirstOrDefault();
                StateHasChanged();
            }
            await base.OnParametersSetAsync();
        }

        protected async Task AddProject()
        {
            if (CurrentBacklogDefinition == null)
                return;

            (CurrentBacklog, CurrentProject) = await BacklogService.CreateProjectInBacklog("Новый проект", CurrentBacklogDefinition, CurrentUnit);
            StateHasChanged();
        }

    }
}
