using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.Two;
using StratML.Core.One;
using System.Threading.Tasks;
using System.Threading;
using StratML.Business.Core;
using StratML.Data.Core;
using StratML.Core;

namespace StratML.Business
{
    public class PartOneLogic : IPartOneLogic
    {
        protected IPartOneDataAdapter Data { get; private set; }
        public PartOneLogic(IPartOneDataAdapter data)
        {
            this.Data = data;
        }
        public Task<IEnumerable<NameId>> GetStrategies(CancellationToken token = default(CancellationToken))
        {
            return this.Data.GetStrategies(token);
        }

        public Task<StrategicPlan> GetStrategy(string id, CancellationToken token = default(CancellationToken))
        {
            return this.Data.GetStrategy(id, token);
        }

        public Task Save(StrategicPlan report, CancellationToken token = default(CancellationToken))
        {
            return this.Data.Save(report, token);
        }
    }
}
