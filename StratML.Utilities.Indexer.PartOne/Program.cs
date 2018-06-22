using System;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using RestSharp;
using System.Threading.Tasks;
using StratML.Core;
using StratML.Core.One;
using System.Collections.Generic;

namespace StratML.Utilities.Indexer.PartOne
{
    class Program
    {
        static void Main(string[] args)
        {
            RestClient client = new RestClient("https://stratml.services/api/");
            RestRequest get = new RestRequest("PartOne", Method.GET);
            var docs = client.Get<List<NameId>>(get);
            foreach(var doc in docs.Data)
            {
                RestRequest patch = new RestRequest("PartOne/{id}", Method.PATCH);
                patch.AddParameter("id", doc.Id, ParameterType.UrlSegment);
                var resp = client.Patch(patch);
            }
        }
    }
}
