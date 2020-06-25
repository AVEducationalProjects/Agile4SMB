using MongoDB.Bson;

namespace Agile4SMB.Server.Model
{
    public class Account
    {
        public ObjectId Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}