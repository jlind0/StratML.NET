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
using StratML.Web.Formatters;

namespace StratML.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            var appconfig = builder.Build();

            var token = new CosmosDataToken(
                new Uri(appconfig["CosmosDB:Path"]),
                appconfig["CosmosDB:Key"],
                appconfig["CosmosDB:Database"],
                appconfig["CosmosDB:Collection"]);
            services.AddSingleton(token);
            services.AddTransient<ICorporationAdapater, CorporationDataAdapter>();
            services.AddTransient<ICorporationLogic, CorporationLogic>();
            
            services.AddMvc(options =>
            {

                options.OutputFormatters.Clear();
                //options.InputFormatters.Clear();

                //options.InputFormatters.Add(new XmlSerializerInputFormatter());

                options.OutputFormatters.Add(new StratMLOutputFormatter());
                

                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
            });
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
