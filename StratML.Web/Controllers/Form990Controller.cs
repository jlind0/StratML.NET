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
using Microsoft.Extensions.Configuration;


namespace StratML.Web.Controllers
{

    public class Form990Controller : Controller
    {
        protected Uri ServicesURI { get; private set; }
        protected IConfiguration Config { get; private set; }
        public Form990Controller(Uri servicesUri, IConfiguration config)
        {
            this.ServicesURI = servicesUri;
            this.Config = config;
        }
        public IActionResult Index()
        {
            return View(this.ServicesURI);
        }
        [Route("Form990/Peek/{version}")]
        public async Task<IActionResult> PeekAtQueue(string version, CancellationToken cToken = default(CancellationToken))
        {

            var queue = new QueueClient(Config["ServiceBusConnectionString"],
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
            var queue = new QueueClient(Config["ServiceBusConnectionString"],
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
            var cerificateThumbprint = Config["KeyVaultAuthCertThumbprint"];
            var authenticationClientId = Config["KeyVaultAuthClientId"];
            var cert = CertificateHelper.FindCertificateByThumbprint(cerificateThumbprint);
            var assertionCert = new ClientAssertionCertificate(authenticationClientId, cert);
            var loginContext = new AuthenticationContext("https://login.microsoftonline.com/88c25c7a-38aa-45d5-bd8d-e939dd68c4f2");
            var result = await loginContext.AcquireTokenAsync(
                "https://management.core.windows.net/", assertionCert
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