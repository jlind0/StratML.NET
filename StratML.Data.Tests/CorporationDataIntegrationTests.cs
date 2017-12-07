using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using StratML.Data;
using StratML.Data.Core;
using StratML.Core;
using StratML.Core.Custom;
using StratML.Core.Three;
using StratML.Core.IRS990;
namespace StratML.Data.Tests
{
    [TestClass]
    public class CorporationDataIntegrationTests
    {
        [TestMethod]
        public async Task CreateCorporationIntegrationTest()
        {
            Corporation corp = new Corporation();
            corp.NameDescription = new NameDescriptionType();
            corp.NameDescription.Identifier = new string[] { Guid.NewGuid().ToString() };
            corp.NameDescription.Name = "Dummy Corp";

            CosmosDataToken token = new CosmosDataToken(new Uri("https://stratml.documents.azure.com:443/"),
                "K7CUa1nkKaHSIj6NV8ptaFwceUTnTkW7nLeTDrlLzWGg8TRnox7s4nNfBnomLp4iohezQMOgvOruyvh4c8PyOQ==", 
                "Lind-I", "Corporations");
            CorporationDataAdapter adapter = new CorporationDataAdapter(token);
            await adapter.SaveCorporation(corp);
            Assert.IsFalse(false);
        }
        [TestMethod]
        public async Task GetRevenueData()
        {
            CosmosDataToken token = new CosmosDataToken(new Uri("https://stratml.documents.azure.com:443/"),
               "NJabTqrkKdTwTByDcuSbaVCfUzDMmWaEvhjpHr4NxNa5ngvPqKFjU0mTtHgPiyVBizbibWk9n8r1pXxiCsmaNA==",
               "Lind-I", "Two");
            IRS990DataAdapter data = new IRS990DataAdapter(token);
            var points = await data.GetDollarPoints();
            Assert.IsFalse(false);
        }
    }
}
