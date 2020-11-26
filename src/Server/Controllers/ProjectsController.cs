using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProjectsController : ControllerBase
    {
        private readonly IBacklogRepository _backlogRepository;
        private readonly IProjectRepository _projectRepository;

        public ProjectsController(IBacklogRepository backlogRepository, IProjectRepository projectRepository)
        {
            _backlogRepository = backlogRepository;
            _projectRepository = projectRepository;
        }

        [HttpGet(Name = "Projects")]
        public ActionResult<IEnumerable<Project>> Get([FromQuery]Guid? unitId)
        {
            if (!unitId.HasValue)
                return NotFound();
            return Ok(_projectRepository.GetByUnitId(unitId.Value));
        }
        
        [HttpPost]
        public ActionResult Post(CreateProjectDTO param)
        {
            var backlog = _backlogRepository.GetBacklog(param.BacklogId);
            if (backlog == null)
                return NotFound(param.BacklogId);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = param.Name,
                UnitId = param.AssigneeId,
                State = ProjectState.New
            };
            _backlogRepository.CreateProject(backlog, project);

            return CreatedAtRoute("Backlogs", project);
        }

        [HttpPatch]
        public ActionResult Patch(Project project)
        {
            if (project == null)
                return NotFound();

            var storedProject = _backlogRepository.GetProject(project.Id);
            if (storedProject == null)
                return NotFound(project.Id);

            _backlogRepository.UpdateProject(project);

            return NoContent();
        }
    }
}
