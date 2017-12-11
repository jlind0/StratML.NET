using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management;
using System.Configuration;
using System.Xml.Linq;
using System.Text;
using RestSharp;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.ServiceBus.Models;
namespace StratML.Cloud.Services.Form990.Categorizer
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("StratML.Cloud.Services.Form990.Categorizer is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("StratML.Cloud.Services.Form990.Categorizer has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("StratML.Cloud.Services.Form990.Categorizer is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();
            this.Client?.CloseAsync().Wait();
            base.OnStop();

            Trace.TraceInformation("StratML.Cloud.Services.Form990.Categorizer has stopped");
        }
        private QueueClient Client { get; set; }
        private List<string> Queues { get; set; } = new List<string>();
        DateTimeOffset hasExpired = DateTimeOffset.MinValue;
        string accessToken = null;
        private async Task RunAsync(CancellationToken cancellationToken)
        {

            Client = new QueueClient(ConfigurationManager.AppSettings["connectionString"],
              ConfigurationManager.AppSettings["queueName"]);

            this.Queues.Clear();
            
            using (ServiceBusManagementClient sb = new ServiceBusManagementClient(await GetCreds())
            {
                SubscriptionId = "8deda4de-adfb-46c8-bc73-a20c32edc81a"
            })
            {
                IPage<SBQueue> qPage = null;
                do
                {
                    qPage = await sb.Queues.ListByNamespaceAsync("stratml", "stratml", cancellationToken);
                    this.Queues.AddRange(qPage.Select(q => q.Name));
                }
                while (qPage?.NextPageLink != null);
            }
            Client.RegisterMessageHandler(async (msg, token) =>
            {

           
                var url = Encoding.UTF8.GetString(msg.Body);
                RestClient rest = new RestClient("http://s3.amazonaws.com/irs-form-990/");
                var resp = rest.Get(new RestRequest(url));
                XDocument doc = XDocument.Parse(resp.Content.Replace("xsi:schemaLocation=\"http://www.irs.gov/efile\"", "").Replace(
                    "xmlns=\"http://www.irs.gov/efile\"", "").Replace("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("\r\n", ""));
                var version = doc.Root.Attribute("returnVersion").Value.Replace(".", "");
                var type = doc.Root.Element("ReturnHeader")?.Element("ReturnType")?.Value ?? "UNK";
                var queueName = (version + "-" + type).ToLower();
                if (!Queues.Contains(queueName))
                {
                    using (ServiceBusManagementClient sb = new ServiceBusManagementClient(await GetCreds())
                    {
                        SubscriptionId = ConfigurationManager.AppSettings["SubscriptionID"]
                    })
                    {
                        await sb.Queues.CreateOrUpdateAsync("stratml", "stratml", queueName, new SBQueue(), token);
                        Queues.Add(queueName);
                    }
                } 
                var q = new QueueClient(ConfigurationManager.AppSettings["connectionString"], queueName);
                await q.SendAsync(new Message(Encoding.UTF8.GetBytes(url)));
                    await q.CloseAsync();
            },
            new MessageHandlerOptions(evt => Task.FromException(evt.Exception))
            { MaxConcurrentCalls = Environment.ProcessorCount*10, AutoComplete = true });
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(60000);
            }
        }
        protected async Task<TokenCredentials> GetCreds()
        {
            if (hasExpired < DateTime.UtcNow.AddMinutes(-2))
            {
                var context = new AuthenticationContext("https://login.microsoftonline.com/88c25c7a-38aa-45d5-bd8d-e939dd68c4f2");
                var result = await context.AcquireTokenAsync(
                    "https://management.core.windows.net/",
                    new ClientCredential(ConfigurationManager.AppSettings["ClientID"], ConfigurationManager.AppSettings["ClientSecret"])
                );
                accessToken = result.AccessToken;
                hasExpired = result.ExpiresOn;

            }

            TokenCredentials creds = new TokenCredentials(accessToken);
            return creds;
        }
    }
}
