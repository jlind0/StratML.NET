using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.Two;
using System.Threading.Tasks;
using System.Threading;
using StratML.Core.One;
using StratML.Core;

namespace StratML.Business.Core
{
    public interface IPartTwoLogic
    {
        Task Save(PerformancePlanOrReport report, CancellationToken token = default(CancellationToken));
        Task<PerformancePlanOrReport> GetStrategy(string id, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<NameId>> GetStrategies(CancellationToken token = default(CancellationToken));
    }
    public interface IPartOneLogic
    {
        Task Save(StrategicPlan report, CancellationToken token = default(CancellationToken));
        Task<StrategicPlan> GetStrategy(string id, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<NameId>> GetStrategies(CancellationToken token = default(CancellationToken));
    }
}
