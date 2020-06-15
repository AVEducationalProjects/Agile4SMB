using System.Linq;
using Agile4SMB.Client.Services;
using Agile4SMB.Client.Utils;
using Agile4SMB.Shared;
using Microsoft.AspNetCore.Components;


namespace Agile4SMB.Client.Pages.Backlog
{
    public class BacklogBase : ComponentBase, ISelectObserver<BacklogDefinitionDTO>, ISelectObserver<ProjectDTO>
    {
        [Inject] public UserService UserService { get; set; }

        protected BacklogDefinitionDTO CurrentBacklog { get; set; }

        protected ProjectDTO CurrentProject { get; set; }
        
        BacklogDefinitionDTO ISelectObserver<BacklogDefinitionDTO>.Item => CurrentBacklog;
        public void Select(BacklogDefinitionDTO item)
        {
            CurrentProject = null;
            CurrentBacklog = item;
            StateHasChanged();
        }
        
        ProjectDTO ISelectObserver<ProjectDTO>.Item => CurrentProject;
        public void Select(ProjectDTO item)
        {
            CurrentProject = item;
            StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            if (CurrentBacklog == null)
            {
                CurrentBacklog = UserService.CurrentUnit.Backlogs.FirstOrDefault();
                StateHasChanged();
            }
            base.OnParametersSet();
        }

        protected void AddProject()
        {
            if (CurrentBacklog == null)
                return;

            UserService.AddProjectToBacklog(CurrentBacklog);
            StateHasChanged();
        }

    }
}
