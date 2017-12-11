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
using StratML.Core.Two;
using StratML.Core;
namespace StratML.Cloud.Services.Form990.Version2009v13
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("StratML.Cloud.Services.Form990.Version2009v13 is running");

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

            Trace.TraceInformation("StratML.Cloud.Services.Form990.Version2009v13 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("StratML.Cloud.Services.Form990.Version2009v13 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("StratML.Cloud.Services.Form990.Version2009v13 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var context = new AuthenticationContext("https://login.microsoftonline.com/88c25c7a-38aa-45d5-bd8d-e939dd68c4f2");
            var queue = new QueueClient(ConfigurationManager.AppSettings["connectionString"],
              "2009v13-990EZ");
            RestClient rest = new RestClient("http://s3.amazonaws.com/irs-form-990/");



            DateTimeOffset hasExpired = DateTimeOffset.MinValue;
            string accessToken = null;
            queue.RegisterMessageHandler(async (msg, token) =>
            {
                
                var url = Encoding.UTF8.GetString(msg.Body);
                string version = null;
                try
                {
                    var resp = rest.Get(new RestRequest(url));
                    XDocument doc = XDocument.Parse(resp.Content.Replace("xsi:schemaLocation=\"http://www.irs.gov/efile\"", "").Replace(
                        "xmlns=\"http://www.irs.gov/efile\"", "").Replace("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("\r\n", ""));
                    version = doc.Root.Attribute("returnVersion").Value.Replace(".", "");
                    var rtn = doc.Element("Return");
                    PerformancePlanOrReport report = new PerformancePlanOrReport();
                    report.OtherInformation = "FORM-990";
                    report.Name = rtn.Element("ReturnHeader").Element("Filer").Element("Name").Element("BusinessNameLine1").Value + "- FORM 990 " + rtn.Element("ReturnHeader").Element("TaxPeriodEndDate").Value;
                    report.Type = PerformancePlanOrReportType.Performance_Report;
                    report.AdministrativeInformation = new AdministrativeInformationType();
                    report.AdministrativeInformation.EndDate = rtn.Element("ReturnHeader").Element("TaxPeriodEndDate").Value;
                    report.AdministrativeInformation.Identifier = Guid.NewGuid().ToString();
                    report.AdministrativeInformation.Source = "http://s3.amazonaws.com/irs-form-990/" + url;
                    report.AdministrativeInformation.PublicationDate = rtn.Element("ReturnHeader").Element("Timestamp").Value;
                    report.Description = rtn.Element("ReturnHeader").Element("ReturnType").Value;
                    report.StrategicPlanCore = new StrategicPlanCore();
                    report.StrategicPlanCore.Mission = new Mission();
                    report.StrategicPlanCore.Mission.Description = rtn.Element("ReturnData").Element("IRS990EZ").Element("PrimaryExemptPurpose").Value;
                    report.StrategicPlanCore.Mission.Identifier = Guid.NewGuid().ToString();
                    var stakeholders = rtn.Element("ReturnData").Element("IRS990EZ"
                    ).Elements("OfficerDirectorTrusteeKeyEmpl").Select(o => new Stakeholder()
                    {
                        StakeholderTypeType = StakeholderStakeholderTypeType.Person,
                        StakeholderTypeTypeSpecified = true,
                        Name = o.Element("PersonName").Value,
                        Role = new Core.Two.Role[] {
                            new Core.Two.Role()
                            {
                                RoleType = new RoleType[]{RoleType.Performer},
                                Name = o.Element("Title").Value,
                                Description = "Title"
                            }
                        }
                    }).ToArray();
                    report.StrategicPlanCore.Organization = new Organization[]
                    {
                        new Organization()
                        {
                            Identifier = rtn.Element("ReturnHeader").Element("Filer").Element("EIN").Value,
                            Acronym = rtn.Element("ReturnHeader").Element("Filer").Element("NameControl").Value,
                            Name = rtn.Element("ReturnHeader").Element("Filer").Element("Name").Element("BusinessNameLine1").Value,
                            Stakeholder = stakeholders
                        }
                    };


                    report.StrategicPlanCore.Goal = new Goal[]
                    {
                        new Goal()
                        {
                            Identifier = "UID-EmployeeHours",
                            Name = "Employee Hours and Compensation",
                            SequenceIndicator = "2",
                            Objective = rtn.Element("ReturnData").Element("IRS990EZ").Elements("OfficerDirectorTrusteeKeyEmpl").Select(e => new ObjectiveType()
                                {
                                    Identifier = Guid.NewGuid().ToString(),
                                    Name = e.Element("PersonName").Value,
                                    Description = e.Element("Title").Value,
                                    PerformanceIndicator = new PerformanceIndicator[]{
                                        new PerformanceIndicator()
                                    {
                                        PerformanceIndicatorType = PerformanceIndicatorTypeType.Quantitative,
                                        PerformanceIndicatorTypeSpecified = true,
                                        Identifier = Guid.NewGuid().ToString(),
                                        MeasurementInstance = new MeasurementInstance[]
                                        {
                                            new MeasurementInstance()
                                            {
                                                ActualResult = new ActualResult[]
                                                {
                                                   new ActualResult()
                                                   {
                                                        Description = "Average Hours Per Week",
                                                        NumberOfUnits = e.Element("AvgHoursPerWkDevotedToPosition").Value
                                                   }
                                                }
                                            },
                                            new MeasurementInstance()
                                            {
                                                ActualResult = new ActualResult[]
                                                {
                                                    new ActualResult()
                                                    {
                                                        Description = "Compensation",
                                                        NumberOfUnits = e.Element("Compensation").Value
                                                    }
                                                }
                                            }
                                        }
                                    }
                                        }
                                }).ToArray(),
                            Stakeholder = stakeholders

                        }
                    };
                    RestClient client = new RestClient("https://stratml.services/api/PartTwo");
                    var request = new RestRequest(Method.POST)
                    {
                        RequestFormat = DataFormat.Xml
                    };
                    request.AddParameter("application/xml", report.Serialize(), ParameterType.RequestBody);
                    var response = await client.ExecuteTaskAsync(request, token);
                }
                catch(Exception ex)
                {
                    if (version != null)
                    {
                        if (hasExpired < DateTime.UtcNow.AddMinutes(-2))
                        {
                            var result = await context.AcquireTokenAsync(
                                "https://management.core.windows.net/",
                                new ClientCredential(ConfigurationManager.AppSettings["ClientID"], ConfigurationManager.AppSettings["ClientSecret"])
                            );
                            accessToken = result.AccessToken;
                            hasExpired = result.ExpiresOn;

                        }

                        TokenCredentials creds = new TokenCredentials(accessToken);
                        using (ServiceBusManagementClient sb = new ServiceBusManagementClient(creds)
                        {
                            SubscriptionId = ConfigurationManager.AppSettings["SubscriptionID"]
                        })
                        {
                            await sb.Queues.CreateOrUpdateAsync("stratml", "stratml", version + "-errors", new SBQueue());
                            var q = new QueueClient(ConfigurationManager.AppSettings["connectionString"], version + "-errors");
                            await q.SendAsync(new Message(Encoding.UTF8.GetBytes($"{{'url':'{url}','ex':'{ex.ToString()}'}}")));
                        }
                    }

                }
            },
            new MessageHandlerOptions(evt => Task.FromException(evt.Exception))
            { MaxConcurrentCalls = 2, AutoComplete = true });
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(100000);
            }
            await queue.CloseAsync();
        }
    }
}
