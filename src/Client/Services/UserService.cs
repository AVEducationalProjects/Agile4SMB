using Agile4SMB.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agile4SMB.Client.Services
{
    public class UserService
    {

        private readonly OrganizationUnitDTO _rootUnit = new OrganizationUnitDTO
        {
            Id = new Guid("14B7DA88-B31E-4380-8807-868C997D4D45"),
            Name = "Adeptik",
            Backlogs = new List<BacklogDefinitionDTO>
            {
                new BacklogDefinitionDTO {Id = new Guid("00EFDF68-109A-42DB-BBF2-F2C914F3D3D5"), Name = "Основной"}
            },
            Children = new List<OrganizationUnitDTO>
            {
                new OrganizationUnitDTO
                {
                    Id = new Guid("EBA5D061-3A88-4C38-80FC-600B665CB5FB"),
                    Name = "Коммерческий департамент",
                    Backlogs = new List<BacklogDefinitionDTO> {new BacklogDefinitionDTO {Id = new Guid("3FCBF137-035A-4AB9-8288-16B6BDC7C1CC"), Name = "Основной-1"}}
                },
                new OrganizationUnitDTO {Id = new Guid("16CD6157-CB35-4800-881A-3F00A0B7A9E0"), Name = "Технический департамент"},
                new OrganizationUnitDTO {Id = new Guid("3ABBC46F-3243-4E45-855C-5AC702F5ECC8"), Name = "БН \"Сервис\""},
                new OrganizationUnitDTO {Id = new Guid("B3E14931-DD4F-4FE4-91D1-F2F5F8DC1454"), Name = "БН \"Производство\""},
            }
        };

        private readonly List<BacklogDTO> _backlogs = new List<BacklogDTO>
        {
            new BacklogDTO
            {
                Id = new Guid("00EFDF68-109A-42DB-BBF2-F2C914F3D3D5"),
                Projects = new List<ProjectDTO>
                {
                    new ProjectDTO
                    {
                        Id = new Guid("2D752FB1-D454-4574-80F4-4659B8DFE0C3"),
                        Name = "Оценка текущего бэклога ICL (не мобильные приложения)",
                        State = ProjectState.InProgress,
                        UnitId = new Guid("3ABBC46F-3243-4E45-855C-5AC702F5ECC8"),
                        Tasks = new List<TaskDTO>(),
                        Goals = new List<ProjectGoalDTO>()
                    }
                }
            },
            new BacklogDTO
            {
                Id = new Guid("3FCBF137-035A-4AB9-8288-16B6BDC7C1CC"),
                Projects = new List<ProjectDTO>()
            }
        };

        public OrganizationUnitDTO CurrentUnit => _rootUnit;

        public BacklogDTO GetBacklog(BacklogDefinitionDTO backlogDefinition)
        {
            return _backlogs.Single(x => x.Id == backlogDefinition.Id);
        }

        public OrganizationUnitDTO GetOrganizationUnit(in Guid unitId)
        {
            return Find(unitId, _rootUnit);

            static OrganizationUnitDTO Find(Guid id, OrganizationUnitDTO unit)
            {
                if (unit.Id == id)
                    return unit;
                return unit
                    .Children
                    .Select(childUnit => Find(id, childUnit))
                    .FirstOrDefault(result => result != null);
            }
        }

        public void CreateProjectInBacklog(BacklogDefinitionDTO backlogDefinition)
        {
            var backlog = GetBacklog(backlogDefinition);
            ((List<ProjectDTO>)backlog.Projects).Add(
                new ProjectDTO
                {
                    Name = "Новый проект",
                    UnitId = CurrentUnit.Id,
                    Tasks = new List<TaskDTO>(),
                    Goals = new List<ProjectGoalDTO>()
                });
        }

        public IEnumerable<OrganizationUnitDTO> GetOrganizationUnits()
        {
            return FlatternOrganizationUnits(_rootUnit);

            List<OrganizationUnitDTO> FlatternOrganizationUnits(OrganizationUnitDTO unit)
            {
                var result = new List<OrganizationUnitDTO> { unit };

                foreach (var child in unit.Children)
                    result.AddRange(FlatternOrganizationUnits(child));

                return result;
            }
        }

        public static OrganizationUnitDTO AddChildOrganizationUnit(OrganizationUnitDTO unit, string name)
        {
            var newUnit = new OrganizationUnitDTO { Name = name };
            ((IList<OrganizationUnitDTO>)unit.Children).Add(newUnit);
            return newUnit;
        }

        public OrganizationUnitDTO DeleteOrganizationUnit(OrganizationUnitDTO unit)
        {
            if (_rootUnit == unit)
                return _rootUnit;

            var parentUnit = FindParentUnit(_rootUnit, unit);
            ((IList<OrganizationUnitDTO>)parentUnit.Children).Remove(unit);
            return parentUnit;

            static OrganizationUnitDTO FindParentUnit(OrganizationUnitDTO baseUnit, OrganizationUnitDTO unitToFind)
            {
                if (baseUnit.Children.Contains(unitToFind))
                    return baseUnit;

                return baseUnit
                    .Children
                    .Select(child =>
                        FindParentUnit(child, unitToFind))
                    .FirstOrDefault(found => found != null);
            }
        }

        public void DeleteBacklog(BacklogDefinitionDTO backlogToDelete)
        {
            var unit = GetOwner(backlogToDelete, _rootUnit);
            ((IList<BacklogDefinitionDTO>)unit.Backlogs).Remove(backlogToDelete);

            static OrganizationUnitDTO GetOwner(BacklogDefinitionDTO backlog, OrganizationUnitDTO unit)
            {
                if (unit.Backlogs.Contains(backlog))
                    return unit;

                return unit
                    .Children
                    .FirstOrDefault(child => GetOwner(backlog, child) != null);
            }

        }


        public BacklogDefinitionDTO CreateBacklog(OrganizationUnitDTO unit, string name)
        {
            var id = Guid.NewGuid();
            var newDef = new BacklogDefinitionDTO { Id = id, Name = name };
            var newBacklog = new BacklogDTO { Id = id, Projects = new List<ProjectDTO>() };

            _backlogs.Add(newBacklog);
            ((IList<BacklogDefinitionDTO>)unit.Backlogs).Add(newDef);

            return newDef;
        }

        private IList<GoalDTO> _goals = new List<GoalDTO>
        {
            new GoalDTO{Id = new Guid("BB0A6A9D-0F83-461F-A5AE-CBF1FE0B781C"), Name = "Выручка с больших клиентов 20 млн. руб. (2020 год) "},
            new GoalDTO{Id = new Guid("4363A829-82A5-412C-9995-4A9145E92606"), Name = "Выручка с существующих клиентов 15 млн. руб. (2020 год) "},
        };

        public IEnumerable<GoalDTO> GetGoals()
        {
            return _goals;
        }

        public void CreateGoal(string name)
        {
            var id = Guid.NewGuid();
            _goals.Add(new GoalDTO { Id = id, Name = name, Description = String.Empty });
        }

        public void DeleteGoal(GoalDTO goal)
        {
            _goals.Remove(goal);
        }

        public void CreateTaskInProject(ProjectDTO project, string name)
        {
            ((IList<TaskDTO>)project.Tasks).Add(new TaskDTO { Name = name, UnitId = CurrentUnit.Id, Done = false });
        }

        public void DeleteTaskFromProject(ProjectDTO project, TaskDTO task)
        {
            if (project.Tasks.Contains(task))
                ((IList<TaskDTO>)project.Tasks).Remove(task);
        }

        public void DeleteProjectGoal(ProjectDTO project, ProjectGoalDTO goal)
        {
            if (project.Goals.Contains(goal))
                ((IList<ProjectGoalDTO>)project.Goals).Remove(goal);
        }

        public void AddProjectGoal(ProjectDTO project, Guid id, string name)
        {
            ((IList<ProjectGoalDTO>)project.Goals).Add(new ProjectGoalDTO{Id = id, Name = name});
        }
    }
}
