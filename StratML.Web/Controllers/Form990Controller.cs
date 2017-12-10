using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using System.Threading;
using System.Text;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using StratML.Web.Models;


namespace StratML.Web.Controllers
{

    public class Form990Controller : Controller
    {
        protected Uri ServicesURI { get; private set; }
        public Form990Controller(Uri servicesUri)
        {
            this.ServicesURI = servicesUri;
        }
        public IActionResult Index()
        {
            return View(this.ServicesURI);
        }
        [Route("Form990/Peek/{version}")]
        public async Task<IActionResult> PeekAtQueue(string version, CancellationToken cToken = default(CancellationToken))
        {

            var queue = new QueueClient(@"Endpoint=sb://stratml.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VsGdifu7wtG1xIGNkqvFQMe9af4t91ou0AbomJOL1A4=",
              version);
            Message message = null;
            var slim = new SemaphoreSlim(0);
            queue.RegisterMessageHandler(async (msg, token) =>
            {
                message = msg;
                await queue.CloseAsync();
                slim.Release();
            }, new MessageHandlerOptions(evt => Task.FromException(evt.Exception)) { AutoComplete = false });
            await slim.WaitAsync(cToken);
            return Redirect("http://s3.amazonaws.com/irs-form-990/" + Encoding.UTF8.GetString(message.Body));
        }
        [Route("Form990/PeekDead/{version}")]
        public async Task<IActionResult> PeekAtQueueDeadLetter(string version, CancellationToken cToken = default(CancellationToken))
        {
            var queue = new QueueClient(@"Endpoint=sb://stratml.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VsGdifu7wtG1xIGNkqvFQMe9af4t91ou0AbomJOL1A4=",
              EntityNameHelper.FormatDeadLetterPath(version));
            Message message = null;
            var slim = new SemaphoreSlim(0);
            queue.RegisterMessageHandler(async (msg, token) =>
            {
                message = msg;
                await queue.CloseAsync();
                slim.Release();
            }, new MessageHandlerOptions(evt => Task.FromException(evt.Exception)) { AutoComplete = false });
            await slim.WaitAsync(cToken);
            return Redirect("http://s3.amazonaws.com/irs-form-990/" + Encoding.UTF8.GetString(message.Body));
        }
        [Route("Queues")]
        public async Task<IActionResult> QueueCounts(CancellationToken token = default(CancellationToken))
        {
            var context = new AuthenticationContext("https://login.microsoftonline.com/88c25c7a-38aa-45d5-bd8d-e939dd68c4f2");
            var result = await context.AcquireTokenAsync(
                       "https://management.core.windows.net/",
                       new ClientCredential("0f64de74-f77a-4d1b-b234-5fd8a7d7a1ce", "TzO7firWSuqqjhNh5GpULtxHZQy8AP5g0vWYl8OUtik=")
                   );
            var accessToken = result.AccessToken;
            TokenCredentials creds = new TokenCredentials(accessToken);
            List<QueueViewModel> queues = new List<QueueViewModel>();
            using (ServiceBusManagementClient sb = new ServiceBusManagementClient(creds)
            {
                SubscriptionId = "8deda4de-adfb-46c8-bc73-a20c32edc81a"
            })
            {
               
                foreach (var queue in await sb.Queues.ListByNamespaceAsync("stratml", "stratml"))
                {
                    queues.Add(new QueueViewModel()
                    {
                        Name = queue.Name,
                        Count = queue.MessageCount ?? 0
                    });
                }
                
            }
            return View(queues);
        }
    }
}