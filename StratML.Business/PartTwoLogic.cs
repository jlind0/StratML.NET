using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.Two;
using System.Threading.Tasks;
using System.Threading;
using StratML.Business.Core;
using StratML.Data.Core;
using StratML.Core;

namespace StratML.Business
{
    public class PartTwoLogic : IPartTwoLogic
    {
        protected IPartTwoDataAdapter Data { get; private set; }
        public PartTwoLogic(IPartTwoDataAdapter data)
        {
            this.Data = data;
        }
        public Task<IEnumerable<NameId>> GetStrategies(CancellationToken token = default(CancellationToken))
        {
            return this.Data.GetStrategies(token);
        }

        public Task<PerformancePlanOrReport> GetStrategy(string id, CancellationToken token = default(CancellationToken))
        {
            return this.Data.GetStrategy(id, token);
        }

        public Task Save(PerformancePlanOrReport report, CancellationToken token = default(CancellationToken))
        {
            if (report.AdministrativeInformation == null)
                report.AdministrativeInformation = new AdministrativeInformationType();
            if (report.AdministrativeInformation.Identifier == null)
                report.AdministrativeInformation.Identifier = Guid.NewGuid().ToString();
            return this.Data.Save(report, token);
        }
    }
}
