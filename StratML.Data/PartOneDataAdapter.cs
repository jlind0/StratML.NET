using System;
using System.Collections.Generic;
using System.Text;
using StratML.Data.Core;
using StratML.Core.Two;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using StratML.Core.One;
using StratML.Core;

namespace StratML.Data
{
    public class PartOneDataAdapter : CosmosDataAdapter, IPartOneDataAdapter
    {
        public PartOneDataAdapter(CosmosDataToken token) : base(token)
        {
        }

        public async Task<IEnumerable<NameId>> GetStrategies(CancellationToken token = default(CancellationToken))
        {
            List<NameId> corporations = new List<NameId>();
            await UseClient(async client =>
            {
                var query = CreateQuery<StrategicPlan>(client, new FeedOptions() { MaxItemCount = -1 }).Select(c =>
                new NameId() { Name = c.Name, Id = c.id }).AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    corporations.AddRange(await query.ExecuteNextAsync<NameId>(token));
                }

            });
            return corporations;
        }

        public async Task<StrategicPlan> GetStrategy(string id, CancellationToken token = default(CancellationToken))
        {
            List<StrategicPlan> corporations = new List<StrategicPlan>();
            await UseClient(async client =>
            {

                var query = CreateQuery<StrategicPlan>(client, new FeedOptions() { MaxItemCount = 1 }).Where(
                    c => c.id == id).AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    corporations.AddRange(await query.ExecuteNextAsync<StrategicPlan>(token));
                }

            });
            return corporations.SingleOrDefault();
        }

        public Task Save(StrategicPlan report, CancellationToken token = default(CancellationToken))
        {
            return UseClient(client => client.CreateDocumentAsync(this.DataPath, report));
        }
    }
}
