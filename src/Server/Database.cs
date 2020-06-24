using System;
using System.Collections.Generic;
using Agile4SMB.Server.Options;
using Agile4SMB.Shared;
using Agile4SMB.Shared.Domain;
using MongoDB.Driver;

namespace Agile4SMB.Server
{
    public static class Database
    {
        public static void Seed(MongoOptions mongoOptions)
        {
            var client = new MongoClient(mongoOptions.Server);
            var database = client.GetDatabase(mongoOptions.Database);

            SeedOrganization();

            void SeedOrganization()
            {
                var collection = database.GetCollection<OrganizationUnit>(mongoOptions.OrganizationUnitCollection);
                
                if(collection.CountDocuments(_ =>true ) != 0)
                    return;
                collection.InsertOne(new OrganizationUnit
                {
                    Id = new Guid("14B7DA88-B31E-4380-8807-868C997D4D45"),
                    Name = "Компания",
                    UserName = "Admin",
                    Backlogs = new List<BacklogDefinition>
                    {
                        new BacklogDefinition {Id = new Guid("00EFDF68-109A-42DB-BBF2-F2C914F3D3D5"), Name = "Основной"}
                    },
                    Children = new List<OrganizationUnit>
                    {
                        new OrganizationUnit
                        {
                            Id = new Guid("EBA5D061-3A88-4C38-80FC-600B665CB5FB"),
                            Name = "Коммерческий департамент",
                            Backlogs = new List<BacklogDefinition> {new BacklogDefinition {Id = new Guid("3FCBF137-035A-4AB9-8288-16B6BDC7C1CC"), Name = "Основной-1"}}
                        },
                        new OrganizationUnit {Id = new Guid("16CD6157-CB35-4800-881A-3F00A0B7A9E0"), Name = "Технический департамент"},
                        new OrganizationUnit {Id = new Guid("3ABBC46F-3243-4E45-855C-5AC702F5ECC8"), Name = "БН \"Сервис\""},
                        new OrganizationUnit {Id = new Guid("B3E14931-DD4F-4FE4-91D1-F2F5F8DC1454"), Name = "БН \"Производство\""},
                    }
                });
            }
        }

        
    }
}