using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Serialization;
using ServiceClients;
using ServiceStore;

using Swashbuckle.AspNetCore.Swagger;

namespace AggregatorService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            Configuration = config;

            this.hostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        protected IHostingEnvironment hostingEnvironment;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the store as a singleton
            services.AddSingleton<IStore>(new Store());

             // Add the historian client
             services.AddTransient<ITemperatureHistorian>( 
                 (s) => new TemperatureHistorian(new Uri(Configuration["TEMPHISTORIAN"])));

            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(
                    opts =>
                    {
                        opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API V1", Version = "v1" });
                c.EnableAnnotations();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            });
        }
    }
}
