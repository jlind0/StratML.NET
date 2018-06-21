using System;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using RestSharp;
using System.Threading.Tasks;
using StratML.Core;
using StratML.Core.One;

namespace StratML.Utilities.Importer.PartOne
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Directory");
            var dir = Console.ReadLine();
            var files = Directory.GetFiles(dir, "*.xml");
            foreach (var file in files.Where(f => !f.EndsWith("Style.xml")))
            {
                try
                {
                    using (var sr = new StreamReader(file))
                    {
                        RestClient client = new RestClient("https://stratml.services/api/");
                        RestRequest request = new RestRequest("PartOne", Method.POST);
                        request.AddParameter("application/xml", sr.ReadToEnd(), ParameterType.RequestBody);
                        var resp = client.Post(request);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(file);
                    Console.WriteLine(ex);
                }
            }
        }
    }
}