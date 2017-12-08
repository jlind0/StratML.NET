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
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

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


                options.InputFormatters.Insert(0, new XMLHelperInputFormatter());

                options.OutputFormatters.Insert(0, new XMLHelperOutputFormatter());
                options.Filters.Add(new RequireHttpsAttribute());

            }).AddControllersAsServices();
            services.AddCors(options =>
            {
                
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                                                    .AllowAnyMethod()
                                                    .AllowAnyHeader());
            });
            
            var customToken = new CosmosDataToken(
                new Uri(Configuration["CosmosDB:Path"]),
                Configuration["CosmosDB:Key"],
                Configuration["CosmosDB:Database"],
                Configuration["CosmosDB:Collections:Custom"]);
            var twoToken = new CosmosDataToken(
                new Uri(Configuration["CosmosDB:Path"]),
                Configuration["CosmosDB:Key"],
                Configuration["CosmosDB:Database"],
               Configuration["CosmosDB:Collections:Two"]);
            var oneToken = new CosmosDataToken(
                new Uri(Configuration["CosmosDB:Path"]),
                Configuration["CosmosDB:Key"],
                Configuration["CosmosDB:Database"],
                Configuration["CosmosDB:Collections:One"]);
            services.AddSwaggerGen(gen =>
            {
                gen.CustomSchemaIds(x => x.FullName);
                gen.SwaggerDoc("v0.1", new Info() { Title = "StratML API" , Version = "v0.1"});
                
            });
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = MicrosoftAccountDefaults.AuthenticationScheme;
            //}).AddCookie(option =>
            //{
            //    option.Cookie.Name = ".myAuth"; //optional setting
            //}).AddMicrosoftAccount(microsoftOptions =>
            //{
            //    microsoftOptions.ClientId = Configuration["Authentication:AppId"];
            //    microsoftOptions.ClientSecret = Configuration["Authentication:Key"];
                
            //});
            Container container = new Container();
            container.Configure(config =>
            {
                config.For<CosmosDataToken>().Add(customToken).Named("Custom");
                config.For<ICorporationAdapater>().Use<CorporationDataAdapter>().Ctor<CosmosDataToken>().IsNamedInstance("Custom");
                config.For<ICorporationLogic>().Use<CorporationLogic>();

                config.For<CosmosDataToken>().Add(twoToken).Named("Two");
                config.For<IPartTwoDataAdapter>().Use<PartTwoDataAdapter>().Ctor<CosmosDataToken>().IsNamedInstance("Two");
                config.For<IIRS990DataAdapter>().Use<IRS990DataAdapter>().Ctor<CosmosDataToken>().IsNamedInstance("Two");
                config.For<IPartTwoLogic>().Use<PartTwoLogic>();
                config.For<IIRS990Logic>().Use<IRS990Logic>();
                
                config.For<CosmosDataToken>().Add(oneToken).Named("One");
                config.For<IPartOneDataAdapter>().Use<PartOneDataAdapter>().Ctor<CosmosDataToken>().IsNamedInstance("One");
                config.For<IPartOneLogic>().Use<PartOneLogic>();


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
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
               );

            });
            app.UseCors("AllowAll");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v0.1/swagger.json", " StratML API");
               
            });
            var options = new RewriteOptions().AddRedirectToHttps();
            app.UseRewriter(options);
            //app.UseAuthentication();
        }
    }
}
