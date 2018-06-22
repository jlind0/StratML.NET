using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StratML.Core.One;
using StratML.Business.Core;
using System.Threading;
using StratML.Core;


namespace StratML.Web.Services.Controllers
{
    [Route("api/PartOne")]
    public class PartOneController : Controller
    {
        protected IPartOneLogic Logic { get; private set; }
        public PartOneController(IPartOneLogic logic)
        {
            this.Logic = logic;
        }
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] StrategicPlan plan, CancellationToken token = default(CancellationToken))
        {
            await this.Logic.Save(plan, token);
            return Ok();
        }
        [HttpGet]
        public Task<IEnumerable<NameId>> GetStrategies(CancellationToken token = default(CancellationToken))
        {
            return this.Logic.GetStrategies(token);
        }
        [HttpGet("{id}")]
        public Task<StrategicPlan> GetStrategy(string id, CancellationToken token = default(CancellationToken))
        {
            return this.Logic.GetStrategy(id, token);
        }
        [HttpPatch("{id}")]
        public Task UpdateIndexes(string id, CancellationToken token = default(CancellationToken))
        {
            return this.Logic.UpdateIndexes(id, token);
        }
    }
}