using System;
using System.Collections.Generic;
using Agile4SMB.Server.Repositories;
using Agile4SMB.Shared.Domain;
using Agile4SMB.Shared.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agile4SMB.Server.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalRepository _goalRepository;

        public GoalsController(IGoalRepository goalRepository)
        {
            _goalRepository = goalRepository;
        }

        [HttpGet(Name = "Goals")]
        public ActionResult<IEnumerable<Goal>> Get()
        {
            return Ok(_goalRepository.GetAll());
        }

        [HttpPost]
        public ActionResult Post(CreateGoalDTO param)
        {
            var goal = new Goal
            {
                Id = Guid.NewGuid(),
                Name = param.Name,
                Description = param.Description
            };

            _goalRepository.Create(goal);

            return Created("Goals", goal);
        }

        [HttpPatch]
        public ActionResult Patch(Goal goal)
        {
            if (goal == null)
                return NotFound();

            if(_goalRepository.Get(goal.Id) == null)
                return NotFound(goal.Id);

            _goalRepository.Update(goal);

            return Created("Goals", goal);
        }

        [HttpDelete]
        public ActionResult Delete(Guid? id)
        {
            if (!id.HasValue)
                return NotFound();

            var goal = _goalRepository.Get(id.Value);
            if (goal == null)
                return NotFound(id.Value);

            _goalRepository.Delete(goal);

            return NoContent();
        }

    }
}
