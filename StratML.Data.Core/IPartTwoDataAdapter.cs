﻿using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.Two;
using System.Threading.Tasks;
using System.Threading;

namespace StratML.Data.Core
{
    public interface IPartTwoDataAdapter
    {
        Task Save(PerformancePlanOrReport report, CancellationToken token = default(CancellationToken));
        Task<PerformancePlanOrReport> GetStrategy(string id, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<NameId>> GetStrategies(CancellationToken token = default(CancellationToken));
    }

    
}
