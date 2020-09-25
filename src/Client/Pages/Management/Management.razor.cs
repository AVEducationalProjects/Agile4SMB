using System;
using System.Threading.Tasks;
using Agile4SMB.Client.Services;
using Agile4SMB.Client.Utils;
using Agile4SMB.Shared.Domain;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;


namespace Agile4SMB.Client.Pages.Management
{
    public class ManagementBase : ComponentBase, ISelectObserver<OrganizationUnit>
    {
        [Inject] public UserUnitService UserUnitService { get; set; }
        
        protected OrganizationUnit SelectedUnit { get; set; }
        /// <summary>
        /// current logged user
        /// </summary>
        public OrganizationUnit UserUnit { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            UserUnit = await UserUnitService.GetCurrentUnit();
            Update();
        }

        protected override async Task OnParametersSetAsync()
        {
            UserUnit = await UserUnitService.GetCurrentUnit();
        }

        public OrganizationUnit Item => SelectedUnit;

        public async Task Select(OrganizationUnit item)
        {
            UserUnit = await UserUnitService.GetCurrentUnit();
            SelectedUnit = item;
            Update();
        }

        public void Update()
        {
            StateHasChanged();
        }
    }

}
