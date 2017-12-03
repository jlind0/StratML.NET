using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StratML.Core;
using StratML.Business.Core;
using System.Threading;

namespace StratML.Web.Controllers
{
    [Route("api/Corporation")]
    public class CorporationController : Controller
    {
        protected ICorporationLogic CorpLogic { get; private set; }
        public CorporationController(ICorporationLogic corpLogic)
        {
            this.CorpLogic = corpLogic;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken token = default(CancellationToken))
        {
            var corp = await this.CorpLogic.GetCorporation(id, token);
            if (corp != null)
                return Ok(corp);
            return this.NoContent();
        }
    }
}