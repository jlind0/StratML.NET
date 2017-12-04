using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StratML.Business.Core;
using StratML.Data.Core;
using StratML.Core;
using StratML.Data;
using StratML.Business;
using System.IO;
using Microsoft.AspNetCore.Mvc.Formatters;
using StructureMap;

namespace StratML.Web.Services
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {

                options.OutputFormatters.Clear();
                options.InputFormatters.Clear();

                options.InputFormatters.Add(new XmlSerializerInputFormatter());

                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());

            }).AddControllersAsServices();
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            var appconfig = builder.Build();

            var customToken = new CosmosDataToken(
                new Uri(appconfig["CosmosDB:Path"]),
                appconfig["CosmosDB:Key"],
                appconfig["CosmosDB:Database"],
                appconfig["CosmosDB:Collections:Custom"]);
        
            Container container = new Container();
            container.Configure(config =>
            {
                config.For<CosmosDataToken>().Add(customToken).Named("Custom");
                config.For<ICorporationAdapater>().Use<CorporationDataAdapter>().Named("Custom");
                config.For<ICorporationLogic>().Use<CorporationLogic>().Named("Custom");
                
                config.Populate(services);
            });
            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
