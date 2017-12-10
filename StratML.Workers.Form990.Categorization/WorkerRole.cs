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
using Microsoft.Azure.Management.ServiceBus.Models;

namespace StratML.Workers.Form990.Categorization
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("StratML.Workers.Form990.Categorization is running");

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

            Trace.TraceInformation("StratML.Workers.Form990.Categorization has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("StratML.Workers.Form990.Categorization is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("StratML.Workers.Form990.Categorization has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var context = new AuthenticationContext("https://login.microsoftonline.com/88c25c7a-38aa-45d5-bd8d-e939dd68c4f2");
            var queue = new QueueClient(ConfigurationManager.AppSettings["connectionString"],
              ConfigurationManager.AppSettings["queueName"]);
            RestClient rest = new RestClient("http://s3.amazonaws.com/irs-form-990/");



            DateTimeOffset hasExpired = DateTimeOffset.MinValue;
            string accessToken = null;
            queue.RegisterMessageHandler(async (msg, token) => 
            {
                if (hasExpired < DateTime.UtcNow.AddMinutes(-2))
                {
                    var result = await context.AcquireTokenAsync(
                        "https://management.core.windows.net/",
                        new ClientCredential("0f64de74-f77a-4d1b-b234-5fd8a7d7a1ce", "TzO7firWSuqqjhNh5GpULtxHZQy8AP5g0vWYl8OUtik=")
                    );
                    accessToken = result.AccessToken;
                    hasExpired = result.ExpiresOn;

                }
                
                TokenCredentials creds = new TokenCredentials(accessToken);
                ServiceBusManagementClient sb = new ServiceBusManagementClient(creds)
                {
                    SubscriptionId = "8deda4de-adfb-46c8-bc73-a20c32edc81a"
                };
                var url = Encoding.UTF8.GetString(msg.Body);
                var resp = rest.Get(new RestRequest(url));
                XDocument doc = XDocument.Parse(resp.Content.Replace("xsi:schemaLocation=\"http://www.irs.gov/efile\"", "").Replace(
                    "xmlns=\"http://www.irs.gov/efile\"", "").Replace("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("\r\n", ""));
                var version = doc.Root.Attribute("returnVersion").Value.Replace(".","");
                await sb.Queues.CreateOrUpdateAsync("stratml", "stratml", version, new SBQueue());
                var q = new QueueClient(ConfigurationManager.AppSettings["connectionString"], version);
                await q.SendAsync(new Message(Encoding.UTF8.GetBytes(url)));
            },
            new MessageHandlerOptions(evt => Task.FromException(evt.Exception))
            { MaxConcurrentCalls = 10, AutoComplete = true });
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(100000);
            }
        }
    }
}
