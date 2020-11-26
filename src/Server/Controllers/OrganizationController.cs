using System;
using Agile4SMB.Server.Repositories;
using Agile4SMB.Shared.Domain;
using Agile4SMB.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agile4SMB.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {

        private readonly IOrganizationUnitRepository _organizationUnitRepository;

        public OrganizationController(IOrganizationUnitRepository organizationUnitRepository)
        {
            _organizationUnitRepository = organizationUnitRepository;
        }

        
        [HttpGet("{backlogId}")]
        public ActionResult<OrganizationUnit> Get(Guid backlogId)
        {
            return Ok(_organizationUnitRepository.GetOwner(backlogId));
        }

        
        [HttpGet(Name = "Organization")]
        public ActionResult<OrganizationUnit> Get()
        {
            return Ok(_organizationUnitRepository.Get(User.Identity.Name));
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

        [HttpPatch]
        public ActionResult Patch(OrganizationUnit unit)
        {
            if (unit == null)
                return NotFound();

            var unitToPatch = _organizationUnitRepository.Get(unit.Id);
            if (unitToPatch == null)
                return NotFound(unit.Id);

            _organizationUnitRepository.Update(unit);

            return NoContent();
        }
    }
}
