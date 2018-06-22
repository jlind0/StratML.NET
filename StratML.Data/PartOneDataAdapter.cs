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
            return UseClient(async client =>
            {
                await client.CreateDocumentAsync(this.DataPath, report);
                await UpdateIndexes(report.id, token);
            });
        }

        public Task UpdateIndexes(string id, CancellationToken token = default(CancellationToken))
        {
            return UseClient(async client =>
            {
                var strat = await GetStrategy(id, token);
                if(strat != null)
                {
                    strat.organizationAcronymCollection = strat.StrategicPlanCore?.Organization?.BuildCollectionString(o => o.Acronym);
                    strat.organizationDescriptionCollection = strat.StrategicPlanCore?.Organization?.BuildCollectionString(o => o.Description);
                    strat.organizationNameCollection = strat.StrategicPlanCore?.Organization?.BuildCollectionString(o => o.Name);
                    strat.stakeholderDescriptionCollection = strat.StrategicPlanCore?.Goal?.SelectMany(g => g.Stakeholder).Select(s => s.Description).Union(
                        strat.StrategicPlanCore?.Goal?.SelectMany(g => g.Objective).SelectMany(o => o.Stakeholder).Select(s => s.Description)).Union(
                            strat.StrategicPlanCore?.Organization?.SelectMany(s => s.Stakeholder).Select(s => s.Description)).BuildCollectionString(s => s);
                    strat.stakeholderNameCollection = strat.StrategicPlanCore?.Goal?.SelectMany(g => g.Stakeholder).Select(s => s.Name).Union(
                        strat.StrategicPlanCore?.Goal?.SelectMany(g => g.Objective).SelectMany(o => o.Stakeholder).Select(s => s.Name)).Union(
                            strat.StrategicPlanCore?.Organization?.SelectMany(s => s.Stakeholder).Select(s => s.Name)).BuildCollectionString(s => s);
                    strat.valueDescriptionCollection = strat.StrategicPlanCore?.Value?.Select(v => v.Description).BuildCollectionString(v => v);
                    strat.valueNameCollection = strat.StrategicPlanCore?.Value?.Select(v => v.Name).BuildCollectionString(v => v);
                    strat.goalDescriptionCollection = strat.StrategicPlanCore?.Goal?.BuildCollectionString(s => s.Description);
                    strat.goalNameCollection = strat.StrategicPlanCore?.Goal?.BuildCollectionString(s => s.Name);
                    strat.goalOtherInformationCollection = strat.StrategicPlanCore?.Goal?.BuildCollectionString(s => s.OtherInformation);
                    strat.objectiveDescriptionCollection = strat.StrategicPlanCore?.Goal?.SelectMany(g => g.Objective).BuildCollectionString(o => o.Description);
                    strat.objectiveNameCollection = strat.StrategicPlanCore?.Goal?.SelectMany(g => g.Objective).BuildCollectionString(o => o.Name);
                    strat.objectiveOtherInformationCollection = strat.StrategicPlanCore?.Goal?.SelectMany(g => g.Objective).BuildCollectionString(o => o.OtherInformation);

                    await client.UpsertDocumentAsync(this.DataPath, strat);
                }
            });
        }
    }
}
