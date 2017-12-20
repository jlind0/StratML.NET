using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StratML.Core.ThreeFive;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using System.IO;
using StratML.Core;

namespace StratML.Web.Controllers
{
    public class StratML35Controller : Controller
    {
        protected Uri ServicesURI { get; private set; }
        protected IConfiguration Config { get; private set; }
        public StratML35Controller(Uri servicesUri, IConfiguration config)
        {
            this.ServicesURI = servicesUri;
            this.Config = config;
        }
        [HttpGet]
        [Route("StratML35/{fileName}")]
        public async Task<IActionResult> Index(string fileName, CancellationToken token = default(CancellationToken))
        {
            CloudStorageAccount acct = CloudStorageAccount.Parse(Config["StorageAccountConnectionString"]);
            var fileClient = acct.CreateCloudFileClient();
            var awsfiles = fileClient.GetShareReference("stratml-35");
            var root = awsfiles.GetRootDirectoryReference();
            var file = root.GetFileReference(fileName);
            OrganizationalStrategyDocument doc = null;
            if (await file.ExistsAsync())
            {
                using (var streamReader = new StreamReader(await file.OpenReadAsync()))
                {
                    doc = XmlHelper.Deserialize<OrganizationalStrategyDocument>(
                        await streamReader.ReadToEndAsync());
                }
            }
            return View(doc);
        }
    }
}