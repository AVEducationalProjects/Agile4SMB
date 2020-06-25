using System.Threading.Tasks;
using Agile4SMB.Client.Services;
using Agile4SMB.Client.Utils;
using Agile4SMB.Shared.Domain;
using Microsoft.AspNetCore.Components;

namespace Agile4SMB.Client.Pages.Organization
{
    public class OrganizationBase : ComponentBase, ISelectObserver<OrganizationUnit>
    {
        [Inject]
        public UserUnitService UserUnitService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UserUnit = await UserUnitService.GetCurrentUnit();
        }

        public OrganizationUnit UserUnit;

        public OrganizationUnit SelectedUnit { get; set; }

        OrganizationUnit ISelectObserver<OrganizationUnit>.Item => SelectedUnit;

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
