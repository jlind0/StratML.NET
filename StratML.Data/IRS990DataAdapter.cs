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
using StratML.Core.IRS990;

namespace StratML.Data
{
    public class IRS990DataAdapter : CosmosDataAdapter, IIRS990DataAdapter
    {
        public IRS990DataAdapter(CosmosDataToken token) : base(token)
        {
            
        }
        class Form990Metrics
        {
            public string OrganizationId { get; set; }
            public string Name { get; set; }
            public MeasurementInstance[] MeasurementInstance { get; set; }
        }
        public async Task<IEnumerable<IRS990DollarPoints>> GetDollarPoints(string orgId = null, CancellationToken token = default(CancellationToken))
        {
            List<IRS990DollarPoints> points = new List<IRS990DollarPoints>();
            await UseClient(async client =>
            {
                var proc = await client.ExecuteStoredProcedureAsync<Form990Metrics[]>("dbs/2IJ4AA==/colls/2IJ4APh9GgE=/sprocs/2IJ4APh9GgECAAAAAAAAgA==/", orgId);
                points.AddRange(proc.Response.GroupBy(p => p.OrganizationId).Select(g => new IRS990DollarPoints()
                {
                    OrgId = g.Key,
                    Assets = g.SelectMany(p => p.MeasurementInstance).SelectMany(mi => mi.ActualResult).Where(
                        ar => ar.Description.Contains("ASSET AMOUNT")).Select(ar =>
                    {
                        DollarPoint point = new DollarPoint();
                        if (DateTime.TryParse(ar.EndDate, out var endDate))
                            point.AsOfDate = endDate;
                        if (double.TryParse(ar.NumberOfUnits, out var dollars))
                            point.Amount = dollars;
                        return point;
                    }).ToArray(),
                    Income = g.SelectMany(p => p.MeasurementInstance).SelectMany(mi => mi.ActualResult).Where(
                        ar => ar.Description.Contains("INCOME AMOUNT")).Select(ar =>
                    {
                        DollarPoint point = new DollarPoint();
                        if (DateTime.TryParse(ar.EndDate, out var endDate))
                            point.AsOfDate = endDate;
                        if (double.TryParse(ar.NumberOfUnits, out var dollars))
                            point.Amount = dollars;
                        return point;
                    }).ToArray(),
                    Revenue = g.SelectMany(p => p.MeasurementInstance).SelectMany(mi => mi.ActualResult).Where(
                        ar => ar.Description.Contains("FORM 990 REVENUE AMOUNT")).Select(ar =>
                    {
                        DollarPoint point = new DollarPoint();
                        if (DateTime.TryParse(ar.EndDate, out var endDate))
                            point.AsOfDate = endDate;
                        if (double.TryParse(ar.NumberOfUnits, out var dollars))
                            point.Amount = dollars;
                        return point;
                    }).ToArray() 
                }));
            });
            return points;
        }
    }
}
