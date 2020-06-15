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
            Id = 1,
            Name = "Adeptik",
            Backlogs = new List<BacklogDefinitionDTO>
            {
                new BacklogDefinitionDTO {Id = 1, Name = "Основной"}
            },
            Children = new List<OrganizationUnitDTO>
            {
                new OrganizationUnitDTO
                {
                    Id = 11,
                    Name = "Коммерческий департамент",
                    Backlogs = new List<BacklogDefinitionDTO> {new BacklogDefinitionDTO {Id = 2, Name = "Основной-1"}}
                },
                new OrganizationUnitDTO {Id = 2, Name = "Технический департамент"},
                new OrganizationUnitDTO {Id = 3, Name = "БН \"Сервис\""},
                new OrganizationUnitDTO {Id = 4, Name = "БН \"Производство\""},
            }
        };

        private readonly List<BacklogDTO> _backlogs = new List<BacklogDTO>
        {
            new BacklogDTO
            {
                Id = 1,
                Projects = new List<ProjectDTO>
                {
                    new ProjectDTO
                    {
                        Id = 1,
                        Name = "Оценка текущего бэклога ICL (не мобильные приложения)",
                        State = ProjectState.InProgress,
                        UnitId = 3
                    }
                }
            },
            new BacklogDTO
            {
                Id = 2,
                Projects = new List<ProjectDTO>()
            }
        };

        public OrganizationUnitDTO CurrentUnit => _rootUnit;

        public BacklogDTO GetBacklog(BacklogDefinitionDTO backlogDefinition)
        {
            return _backlogs.Single(x => x.Id == backlogDefinition.Id);
        }

        public OrganizationUnitDTO GetOrganizationUnit(in long unitId)
        {
            return Find(unitId, _rootUnit);

            static OrganizationUnitDTO Find(long id, OrganizationUnitDTO unit)
            {
                if (unit.Id == id)
                    return unit;
                return unit
                    .Children
                    .Select(childUnit => Find(id, childUnit))
                    .FirstOrDefault(result => result != null);
            }
        }

        public void AddProjectToBacklog(BacklogDefinitionDTO backlogDefinition)
        {
            var backlog = GetBacklog(backlogDefinition);
            ((List<ProjectDTO>)backlog.Projects).Add(
                new ProjectDTO
                {
                    Name = "Новый проект",
                    UnitId = CurrentUnit.Id
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

        public static void AddChildOrganizationUnit(OrganizationUnitDTO unit, string name)
        {
            ((IList<OrganizationUnitDTO>)unit.Children).Add(new OrganizationUnitDTO { Name = name });
        }
    }
}
