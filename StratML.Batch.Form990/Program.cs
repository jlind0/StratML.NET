using System;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.Azure.ServiceBus;
using System.Text;


namespace StratML.Batch.Form990
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string lastKey = null;
            using (var file = File.Open(Path.Combine(Directory.GetCurrentDirectory(), "last.txt"),
                FileMode.OpenOrCreate))
            {
                using (var streamReader = new StreamReader(file))
                {
                    lastKey = await streamReader.ReadToEndAsync();
                }
            }
            var queue = new QueueClient(ConfigurationManager.AppSettings["connectionString"],
                ConfigurationManager.AppSettings["queueName"]);
            using (var client = new AmazonS3Client(new AnonymousAWSCredentials(),
                Amazon.RegionEndpoint.USEast1))
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
                    response = await client.ListObjectsV2Async(request);

                    foreach (var obj in response.S3Objects)
                    {
                        await queue.SendAsync(new Message(Encoding.UTF8.GetBytes(obj.Key)));
                        lastKey = obj.Key;
                    }
                    request.ContinuationToken = response.NextContinuationToken;
                }
                while (response.IsTruncated);
                await File.WriteAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "last.txt"), lastKey);
            }
        }
    }
}
