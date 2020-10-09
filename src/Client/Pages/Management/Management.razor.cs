using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agile4SMB.Client.Services;
using Agile4SMB.Client.Utils;
using Agile4SMB.Shared.Domain;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;


namespace Agile4SMB.Client.Pages.Management
{
    public class ManagementBase : ComponentBase, ISelectObserver<OrganizationUnit>, ISelectObserver<Project>
    {
        [Inject] public UserUnitService UserUnitService { get; set; }
        
        protected OrganizationUnit SelectedUnit { get; set; }
        protected Project SelectedProject { get; set; }
        /// <summary>
        /// current logged user
        /// </summary>
        protected OrganizationUnit CurrentUnit { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            CurrentUnit = await UserUnitService.GetCurrentUnit();
            Update();
        }

        protected override void OnParametersSet()
        {
            Update();
            base.OnParametersSet();
        }


        OrganizationUnit ISelectObserver<OrganizationUnit>.Item => SelectedUnit;

        public Task Select(OrganizationUnit item)
        {
            SelectedUnit = item;
            Update();
            return Task.CompletedTask;
        }

        Project ISelectObserver<Project>.Item => SelectedProject;
        
        public Task Select(Project item)
        {
            SelectedProject = item;
            Update();
            return Task.CompletedTask;
        }

        public void Update()
        {
            StateHasChanged();
        }
        
    }

}
