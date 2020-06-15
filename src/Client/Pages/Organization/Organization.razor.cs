using Agile4SMB.Client.Utils;
using Agile4SMB.Shared;
using Microsoft.AspNetCore.Components;

namespace Agile4SMB.Client.Pages.Organization
{
    public class OrganizationBase : ComponentBase, ISelectObserver<OrganizationUnitDTO>
    {
        public OrganizationUnitDTO SelectedUnit { get; set; }

        OrganizationUnitDTO ISelectObserver<OrganizationUnitDTO>.Item => SelectedUnit;

        public void Select(OrganizationUnitDTO item)
        {
            SelectedUnit = item;
            StateHasChanged();
        }
    }
}
