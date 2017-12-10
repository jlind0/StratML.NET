using System;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.Net;

namespace Form990AWSEnqueue
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("Form990AWSEnqueue is running");

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

            Trace.TraceInformation("Form990AWSEnqueue has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Form990AWSEnqueue is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Form990AWSEnqueue has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            //string lastKey = null;
            //using (var file = File.Open(Path.Combine(Directory.GetCurrentDirectory(), "last.txt"),
            //    FileMode.OpenOrCreate))
            //{
            //    using (var streamReader = new StreamReader(file))
            //    {
            //        lastKey = await streamReader.ReadToEndAsync();
            //    }
            //}
            //var queue = new QueueClient(ConfigurationManager.AppSettings["connectionString"],
            //    ConfigurationManager.AppSettings["queueName"]);
            //using (var client = new AmazonS3Client(new AnonymousAWSCredentials(),
            //    Amazon.RegionEndpoint.USEast1))
            //{
            //    try
            //    {
            //        ListObjectsV2Request request = new ListObjectsV2Request()
            //        {
            //            BucketName = "irs-form-990",
            //            MaxKeys = 100,
            //            StartAfter = string.IsNullOrWhiteSpace(lastKey) ? null : lastKey
            //        };
            //        ListObjectsV2Response response;
            //        do
            //        {
            //            response = await client.ListObjectsV2Async(request, cancellationToken);

            //            foreach (var obj in response.S3Objects)
            //            {
            //                await queue.SendAsync(new Message(Encoding.UTF8.GetBytes(obj.Key)));
            //                lastKey = obj.Key;
            //            }
            //            request.ContinuationToken = response.NextContinuationToken;
            //        }
            //        while (response.IsTruncated);
            //    }
            //    catch { throw; }
            //    finally
            //    {
            //        await queue.CloseAsync();
            //        File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "last.txt"), lastKey);
            //    }
            //}
        }
    }
}
