using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agile4SMB.Server.Repositories;
using Agile4SMB.Shared.Domain;
using Agile4SMB.Shared.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agile4SMB.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BacklogsController : ControllerBase
    {
        private readonly IOrganizationUnitRepository _organizationUnitRepository;
        private readonly IBacklogRepository _backlogRepository;

        public BacklogsController(IOrganizationUnitRepository organizationUnitRepository, IBacklogRepository backlogRepository)
        {
            _organizationUnitRepository = organizationUnitRepository;
            _backlogRepository = backlogRepository;
        }

        [HttpGet(Name = "Backlogs")]
        public ActionResult<Backlog> Get([FromQuery]Guid? id)
        {
            if (!id.HasValue)
                return NotFound();

            return _backlogRepository.Get(id.Value);
        }

        [HttpPost]
        public ActionResult Post(CreateBacklogDTO param)
        {
            var unit = _organizationUnitRepository.Get(param.UnitId);
            if (unit == null)
                return NotFound(param.UnitId);

            var createdBacklogDefinition = new BacklogDefinition { Id = Guid.NewGuid(), Name = param.Name };
            _backlogRepository.CreateBacklog(unit, createdBacklogDefinition);
            return CreatedAtRoute("Organization", createdBacklogDefinition);
        }

        [HttpPatch]
        public ActionResult Patch(BacklogDefinition backlog)
        {
            if (backlog == null)
                return NotFound();

            _backlogRepository.Update(backlog);

            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete([FromQuery]Guid? id)
        {
            if (!id.HasValue)
                return NotFound();

            var backlog = _backlogRepository.Get(id.Value);
            if (backlog == null)
                return NotFound(id.Value);

            _backlogRepository.Delete(id.Value);

            return NoContent();
        }
    }
}
