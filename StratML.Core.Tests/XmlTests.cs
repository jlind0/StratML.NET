using Microsoft.VisualStudio.TestTools.UnitTesting;
using StratML.Core;
using StratML.Core.Two;
using System.IO;
using System.Threading.Tasks;
namespace StratML.Core.Tests
{
    [TestClass]
    public class XmlTests
    {
        [TestMethod]
        public async Task DeserializeTwoTest()
        {
            var directory = Directory.GetCurrentDirectory();
            var path = Path.Combine(directory, "StratTest.xml");
            using (var stream = File.OpenText(path))
            {
               var data = await stream.ReadToEndAsync();
                Assert.IsNotNull(XmlHelper.Deserialize<PerformancePlanOrReport>(data));

            }
                
        }
    }
}
