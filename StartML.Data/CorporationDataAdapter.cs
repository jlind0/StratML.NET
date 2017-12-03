using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StratML.Core;
using StratML.Data.Core;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace StratML.Data
{
    public class CorporationDataAdapter : CosmosDataAdapter, ICorporationAdapater
    {
        public CorporationDataAdapter(CosmosDataToken token) : base(token) { }

        public async Task<Corporation> GetCorporation(string[] id, CancellationToken token = default(CancellationToken))
        {
            List<Corporation> corporations = new List<Corporation>();
            await UseClient(async client =>
            {
                string flattenedId = string.Join(',', id);
                var query = CreateQuery<Corporation>(client, new FeedOptions() { MaxItemCount = 1 }).AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    corporations.AddRange(await query.ExecuteNextAsync<Corporation>(token));
                }

            });
            return corporations.FirstOrDefault();
        }

        public async Task<ICollection<NameDescriptionType>> GetCorporations(CancellationToken token = default(CancellationToken))
        {
            List<NameDescriptionType> corporations = new List<NameDescriptionType>();
            await UseClient(async client =>
            {
                var query = CreateQuery<Corporation>(client, new FeedOptions() { MaxItemCount = -1 }).Select(c => 
                    c.NameDescription).OrderBy(n => n.Name).AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    corporations.AddRange(await query.ExecuteNextAsync<NameDescriptionType>(token));
                }

            });
            return corporations;
        }

        public Task SaveCorporation(Corporation corporation, CancellationToken token = default(CancellationToken))
        {
            return UseClient(client => client.UpsertDocumentAsync(this.DataPath, corporation));
        }
    }
}
