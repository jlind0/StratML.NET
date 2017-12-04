using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StratML.Business.Core;
using StratML.Core.Two;
using System.Threading;

namespace StratML.Web.Services.Controllers
{
    [Route("api/PartTwo")]
    public class PartTwoController : Controller
    {
        protected IPartTwoLogic Logic { get; private set; }
        public PartTwoController(IPartTwoLogic logic)
        {
            this.Logic = logic;
        }
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] PerformancePlanOrReport plan, CancellationToken token = default(CancellationToken))
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
        public Task<PerformancePlanOrReport> GetStrategy(string id, CancellationToken token = default(CancellationToken))
        {
            return this.Logic.GetStrategy(id, token);
        }
    }
}