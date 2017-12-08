using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StratML.Business.Core;
using StratML.Core.Two;
using System.Threading;
using StratML.Core.One;
using StratML.Transform.Core;
using StratML.Core;
using StratML.Core.IRS990;

namespace StratML.Web.Services.Controllers
{
    [Route("api/IRS990")]
    public class IRS990Controller : Controller
    {
        protected IIRS990Logic Logic { get; private set; }
        public IRS990Controller(IIRS990Logic logic)
        {
            this.Logic = logic;
        }
        [HttpGet("{id}")]
        [HttpGet]
        public Task<IEnumerable<IRS990DollarPoints>> GetDollars(string id = null, CancellationToken token = default(CancellationToken))
        {
            return this.Logic.GetDollarPoints(id, token);
        }
        [HttpGet("organizations")]
        public Task<IEnumerable<NameId>> GetOrgnaizations(CancellationToken token = default(CancellationToken))
        {
            return this.Logic.GetOrganizations(token);
        }
    }
}