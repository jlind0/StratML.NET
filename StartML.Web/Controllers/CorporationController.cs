using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StratML.Core;
using StratML.Business.Core;
using System.Threading;
using StratML.Core.Custom;

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
        public Task<Corporation> Get(string id, CancellationToken token = default(CancellationToken))
        {
            return this.CorpLogic.GetCorporation(id, token);
        }
    }
}