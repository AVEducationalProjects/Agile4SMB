using System.Threading.Tasks;
using Agile4SMB.Client.Services;
using Agile4SMB.Client.Utils;
using Agile4SMB.Shared;
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
            CurrentUnit = await UserUnitService.GetCurrentUnit();
        }

        public OrganizationUnit CurrentUnit;

        public OrganizationUnit SelectedUnit { get; set; }

        OrganizationUnit ISelectObserver<OrganizationUnit>.Item => SelectedUnit;

        public void Select(OrganizationUnit item)
        {
            SelectedUnit = item;
            Update();
        }

        public void Update()
        {
            StateHasChanged();
        }
    }
}
