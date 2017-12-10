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

namespace StratML.Cloud.Services.Form990.AWSEnqueue
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("StratML.Cloud.Services.Form990.AWSEnqueue is running");

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

            Trace.TraceInformation("StratML.Cloud.Services.Form990.AWSEnqueue has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("StratML.Cloud.Services.Form990.AWSEnqueue is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("StratML.Cloud.Services.Form990.AWSEnqueue has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            string lastKey = null;
            CloudStorageAccount acct = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["fileConnectionString"]);
            var fileClient = acct.CreateCloudFileClient();
            var awsfiles = fileClient.GetShareReference("aws990");
            var root = awsfiles.GetRootDirectoryReference();
            var file = root.GetFileReference("last.txt");
            if (await file.ExistsAsync(cancellationToken))
            {
                using (var streamReader = new StreamReader(await file.OpenReadAsync(cancellationToken)))
                {
                    lastKey = (await streamReader.ReadToEndAsync()).Trim('\0');
                }
            }
            var queue = new QueueClient(ConfigurationManager.AppSettings["connectionString"],
                ConfigurationManager.AppSettings["queueName"]);
            using (var client = new AmazonS3Client(new AnonymousAWSCredentials(),
                Amazon.RegionEndpoint.USEast1))
            {
                try
                {
                    ListObjectsV2Request request = new ListObjectsV2Request()
                    {
                        BucketName = "irs-form-990",
                        MaxKeys = 100,
                        StartAfter = string.IsNullOrWhiteSpace(lastKey) ? null : lastKey
                    };
                    ListObjectsV2Response response;
                    do
                    {
                        response = await client.ListObjectsV2Async(request, cancellationToken);

                        foreach (var obj in response.S3Objects)
                        {
                            await queue.SendAsync(new Message(Encoding.UTF8.GetBytes(obj.Key)));
                            lastKey = obj.Key;
                        }
                        request.ContinuationToken = response.NextContinuationToken;
                    }
                    while (response.IsTruncated);
                }
                catch { throw; }
                finally
                {
                    await queue.CloseAsync();
                    if (await file.ExistsAsync())
                        await file.DeleteAsync();
                    else
                        await file.CreateAsync(1024);
                    using (var stream = await file.OpenWriteAsync(1024))
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            await writer.WriteAsync(lastKey);
                        }
                    }
                }
            }
        }
    }
}
