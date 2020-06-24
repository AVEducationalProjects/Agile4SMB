using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Agile4SMB.Server.Options
{
    public class JWTOptions
    {
        public string SigningKey { get; set; }

        public SymmetricSecurityKey GetSigningKey() => new SymmetricSecurityKey(Encoding.UTF32.GetBytes(SigningKey));
    }
}
