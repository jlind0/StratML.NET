using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.One;
using System.Threading.Tasks;
using System.Threading;
using StratML.Core;

namespace StratML.Data.Core
{
    public interface IPartOneDataAdapter
    {
        Task Save(StrategicPlan report, CancellationToken token = default(CancellationToken));
        Task<StrategicPlan> GetStrategy(string id, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<NameId>> GetStrategies(CancellationToken token = default(CancellationToken));
        Task UpdateIndexes(string id, CancellationToken token = default(CancellationToken));
    }
}
