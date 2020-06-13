using System.Linq;
using Agile4SMB.Client.Services;
using Agile4SMB.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;


namespace Agile4SMB.Client.Pages.Backlog
{
    public class BacklogBase : ComponentBase
    {
        [Inject] public UserService UserService { get; set; }

        public BacklogDefinitionDTO CurrentBacklog { get; set; }

        public ProjectDTO CurrentProject { get; set; }

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
