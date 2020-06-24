using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agile4SMB.Server.Repositories;
using Agile4SMB.Shared;
using Agile4SMB.Shared.Domain;
using Agile4SMB.Shared.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agile4SMB.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {

        private readonly IOrganizationUnitRepository _organizationUnitRepository;

        public OrganizationController(IOrganizationUnitRepository organizationUnitRepository)
        {
            _organizationUnitRepository = organizationUnitRepository;
        }

        [HttpGet(Name = "Organization")]
        public ActionResult<OrganizationUnit> Get()
        {
            return Ok(_organizationUnitRepository.Get("Admin"));
        }

        [HttpPost]
        public ActionResult Post(CreateOrganizationUnitDTO param)
        {
            var parent = _organizationUnitRepository.Get(param.ParentId);
            if (parent == null)
                return NotFound(param.ParentId);

            var createdUnit = new OrganizationUnit { Id = Guid.NewGuid(), Name = param.Name };
            _organizationUnitRepository.AddToParent(parent, createdUnit);
            return CreatedAtRoute("Organization", createdUnit);
        }

        [HttpDelete]
        public ActionResult Delete([FromQuery]Guid? id)
        {
            if (!id.HasValue)
                return NotFound();

            var unit = _organizationUnitRepository.Get(id.Value);
            if (unit == null)
                return NotFound(id.Value);

            _organizationUnitRepository.Delete(unit);

            return NoContent();
        }
    }
}
