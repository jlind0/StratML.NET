using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.Two;
using System.Threading.Tasks;
using System.Threading;
using StratML.Business.Core;
using StratML.Data.Core;
using StratML.Core;
using StratML.Core.IRS990;

namespace StratML.Business
{
    public class IRS990Logic : IIRS990Logic
    {
        protected IIRS990DataAdapter DataAdapter { get; private set;}
        public IRS990Logic(IIRS990DataAdapter adapter)
        {
            this.DataAdapter = adapter;
        }
        public Task<IEnumerable<IRS990DollarPoints>> GetDollarPoints(string orgId = null, CancellationToken token = default(CancellationToken))
        {
            return this.DataAdapter.GetDollarPoints(orgId, token);
        }

        public Task<IEnumerable<NameId>> GetOrganizations(CancellationToken token = default(CancellationToken))
        {
            return this.DataAdapter.GetOrganizations(token);
        }
    }
}
