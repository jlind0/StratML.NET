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
using StratML.Core;

namespace StratML.Data
{
    public class PartTwoDataAdapter : CosmosDataAdapter, IPartTwoDataAdapter
    {
        public PartTwoDataAdapter(CosmosDataToken token) : base(token)
        {
        }

        public async Task<IEnumerable<NameId>> GetStrategies(CancellationToken token = default(CancellationToken))
        {
            List<NameId> corporations = new List<NameId>();
            await UseClient(async client =>
            {
                var query = CreateQuery<PerformancePlanOrReport>(client, new FeedOptions() { MaxItemCount = -1 }).Select(c =>
                new NameId() { Name = c.Name, Id = c.AdministrativeInformation.Identifier }).AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    corporations.AddRange(await query.ExecuteNextAsync<NameId>(token));
                }

            });
            return corporations;
        }

        public async Task<PerformancePlanOrReport> GetStrategy(string id, CancellationToken token = default(CancellationToken))
        {
            List<PerformancePlanOrReport> corporations = new List<PerformancePlanOrReport>();
            await UseClient(async client =>
            {

                var query = CreateQuery<PerformancePlanOrReport>(client, new FeedOptions() { MaxItemCount = 1 }).Where(
                    c => c.AdministrativeInformation.Identifier == id).AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    corporations.AddRange(await query.ExecuteNextAsync<PerformancePlanOrReport>(token));
                }

            });
            return corporations.SingleOrDefault();
        }

        public Task Save(PerformancePlanOrReport report, CancellationToken token = default(CancellationToken))
        {
            return UseClient(async client => 
            {
                report.id = await GetCosmosId(report, token);
                await client.UpsertDocumentAsync(this.DataPath, report);
            });
        }
        protected async Task<string> GetCosmosId(PerformancePlanOrReport report, CancellationToken token = default(CancellationToken))
        {
            List<string> ids = new List<string>();
            await UseClient(async client =>
            {
                var query = CreateQuery<PerformancePlanOrReport>(client, new FeedOptions() { MaxItemCount = 1 }).Where(
                       c => c.AdministrativeInformation.Identifier == report.AdministrativeInformation.Identifier).Select(c => c.id).AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    ids.AddRange(await query.ExecuteNextAsync<string>(token));
                }
            });
            return ids.SingleOrDefault();
        }
    }
}
