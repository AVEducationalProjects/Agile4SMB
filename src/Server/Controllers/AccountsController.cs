using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Agile4SMB.Server.Model;
using Agile4SMB.Server.Repositories;
using Agile4SMB.Shared.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Agile4SMB.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IOrganizationUnitRepository _organizationUnitRepository;

        public AccountsController(IOrganizationUnitRepository organizationUnitRepository)
        {
            _organizationUnitRepository = organizationUnitRepository;
        }

        static string Hash(string input)
        {
            using SHA1Managed sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        public ActionResult Post(SetAccountDTO param)
        {
            var unit = _organizationUnitRepository.Get(param.UnitId);
            if (unit == null)
                return NotFound(param.UnitId);

            var account = new Account
            {
                UserName = param.UserName, 
                PasswordHash = Hash(param.Password)
            };

            _organizationUnitRepository.SetAccount(unit, account);

            return Ok();
        }
    }
}
