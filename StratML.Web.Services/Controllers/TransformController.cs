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

namespace StratML.Web.Services.Controllers
{
    [Route("api/Transform")]
    public class TransformController : Controller
    {
        protected IPartTwoLogic Logic { get; private set; }
        protected ITransformTwoToOne Transform { get; private set; }
        protected ITransformOneToTwo TransformOne { get; private set; }
        protected IPartOneLogic LogicOne { get; private set; }
        public TransformController(IPartTwoLogic logic, ITransformTwoToOne twoToOne, ITransformOneToTwo oneToTwo, IPartOneLogic logicOne)
        {
            this.Logic = logic;
            this.Transform = twoToOne;
            this.TransformOne = oneToTwo;
            this.LogicOne = logicOne;
        }
        [HttpPost("two/one")]
        public async Task<StrategicPlan> TransformPlan([FromBody] PerformancePlanOrReport plan, bool persist = false, CancellationToken token = default(CancellationToken))
        {
            var strat = this.Transform.Transform(plan);
            if (persist)
                await this.LogicOne.Save(strat, token);
            return strat;
        }
        [HttpPost("one/two")]
        public async Task<PerformancePlanOrReport> TransformToPlan([FromBody] StrategicPlan strategy, bool persist = false, CancellationToken token = default(CancellationToken))
        {
            var plan = TransformOne.Transform(strategy);
            if (persist)
                await this.Logic.Save(plan, token);
            return plan;
        }
    }
}