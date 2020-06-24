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

        protected BacklogDefinition CurrentBacklog { get; set; }

        protected Project CurrentProject { get; set; }
        
        BacklogDefinition ISelectObserver<BacklogDefinition>.Item => CurrentBacklog;
        public Task Select(BacklogDefinition item)
        {
            CurrentProject = null;
            CurrentBacklog = item;
            Update();

            return Task.CompletedTask;
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
            if (CurrentBacklog == null)
            {
                var unit = await UserUnitService.GetCurrentUnit();
                CurrentBacklog = unit.Backlogs.FirstOrDefault();
                StateHasChanged();
            }
            await base.OnParametersSetAsync();
        }

        protected async Task AddProject()
        {
            if (CurrentBacklog == null)
                return;

            await BacklogService.CreateProjectInBacklog(CurrentBacklog);
            StateHasChanged();
        }

    }
}
