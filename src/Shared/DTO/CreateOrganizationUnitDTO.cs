using System;

namespace Agile4SMB.Shared.DTO
{
    public class CreateOrganizationUnitDTO
    {
        public string Name { get; set; }

        public Guid ParentId { get; set; }
    }
}
