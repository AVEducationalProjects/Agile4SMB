using System;
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
        protected OrganizationUnit UserUnit { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            UserUnit = await UserUnitService.GetCurrentUnit();
            Update();
        }

        protected override async Task OnParametersSetAsync()
        {
            UserUnit = await UserUnitService.GetCurrentUnit();
            Update();
        }

        public OrganizationUnit Item => SelectedUnit;

        public async Task Select(OrganizationUnit item)
        {
            UserUnit = await UserUnitService.GetCurrentUnit();
            SelectedUnit = item;
            Update();
        }

        Project ISelectObserver<Project>.Item => SelectedProject;

        public Task Select(Project item)
        {
            SelectedProject = item;
            Update();
            return null;
        }

        public void Update()
        {
            StateHasChanged();
        }
        
    }

}
