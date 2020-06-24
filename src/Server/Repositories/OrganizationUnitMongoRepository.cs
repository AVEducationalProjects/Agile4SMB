using System;
using System.Linq;
using Agile4SMB.Client;
using Agile4SMB.Server.Model;
using Agile4SMB.Server.Options;
using Agile4SMB.Shared;
using Agile4SMB.Shared.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Agile4SMB.Server.Repositories
{
    public class OrganizationUnitMongoRepository : IOrganizationUnitRepository
    {
        private readonly IMongoCollection<OrganizationUnit> _organizationUnitCollection;
        private readonly IMongoCollection<Account> _accountCollection;

        public OrganizationUnitMongoRepository(IOptions<MongoOptions> mongoOptions)
        {
            var client = new MongoClient(mongoOptions.Value.Server);
            var database = client.GetDatabase(mongoOptions.Value.Database);

            _organizationUnitCollection = database.GetCollection<OrganizationUnit>(mongoOptions.Value.OrganizationUnitCollection);
            _accountCollection = database.GetCollection<Account>(mongoOptions.Value.AccountCollection);
        }

        public OrganizationUnit Get(string username) => 
            _organizationUnitCollection.FindSync(_=>true)
                .Single()
                .Find(username);

        public OrganizationUnit Get(Guid id) =>
            _organizationUnitCollection
                .FindSync(_ => true)
                .Single()
                .Find(id);

        public void AddToParent(OrganizationUnit parent, OrganizationUnit child)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var parentFromRoot = root.Find(parent.Id);

            if (parentFromRoot == null)
                new ApplicationException("Organization unit not found");

            parentFromRoot.Children = parentFromRoot.Children.Union(new[] {child}).ToArray();

            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);
        }

        public void Delete(OrganizationUnit unit)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var parent = root.FindParent(unit.Id);
            parent.Children = parent.Children.Where(x => x.Id != unit.Id).ToArray();
            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);
        }

        public void Update(OrganizationUnit unit)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var updatedUnit = root.Find(unit.Id);
            updatedUnit.Name = unit.Name;
            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);
        }

        public void SetAccount(OrganizationUnit unit, Account account)
        {
            var root = _organizationUnitCollection.FindSync(_ => true).Single();
            var updatedUnit = root.Find(unit.Id);

            if(unit.UserName!=updatedUnit.UserName &&
               _accountCollection.CountDocuments(x=>x.UserName==account.UserName) != 0)
                throw new ApplicationException("Account already exist");

            if (!String.IsNullOrEmpty(updatedUnit.UserName))
                _accountCollection.FindOneAndDelete(x => x.UserName == updatedUnit.UserName);

            _accountCollection.InsertOne(account);

            updatedUnit.UserName = account.UserName;
            _organizationUnitCollection.ReplaceOne(x => x.Id == root.Id, root);
        }
    }
}