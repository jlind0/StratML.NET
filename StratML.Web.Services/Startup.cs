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
using StratML.Transform;
using StratML.Transform.Core;
using StratML.Web.Services.Formatters;

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

                options.InputFormatters.Add(new XMLHelperInputFormatter());

                options.OutputFormatters.Add(new XMLHelperOutputFormatter());

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
            var twoToken = new CosmosDataToken(
                new Uri(appconfig["CosmosDB:Path"]),
                appconfig["CosmosDB:Key"],
                appconfig["CosmosDB:Database"],
                appconfig["CosmosDB:Collections:Two"]);

            Container container = new Container();
            container.Configure(config =>
            {
                config.For<CosmosDataToken>().Add(customToken).Named("Custom");

                config.For<ICorporationAdapater>().Use<CorporationDataAdapter>().Ctor<CosmosDataToken>().IsNamedInstance("Custom");
                config.For<ICorporationLogic>().Use<CorporationLogic>();

                config.For<CosmosDataToken>().Add(twoToken).Named("Two");
                config.For<IPartTwoDataAdapter>().Use<PartTwoDataAdapter>().Ctor<CosmosDataToken>().IsNamedInstance("Two");
                config.For<IPartTwoLogic>().Use<PartTwoLogic>();
                config.For<ITransformOneToTwo>().Use<TransformOneToTwo>();
                config.For<ITransformTwoToOne>().Use<TransformTwoToOne>();
                
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
