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
    public class ProjectsController : ControllerBase
    {
        private readonly IBacklogRepository _backlogRepository;

        public ProjectsController(IBacklogRepository backlogRepository)
        {
            _backlogRepository = backlogRepository;
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
