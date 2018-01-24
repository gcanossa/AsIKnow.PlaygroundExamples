using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Winton.Extensions.Configuration.Consul;

namespace WebApplication1
{
    //docker run -d --name=dev-consul -e CONSUL_BIND_INTERFACE=eth0 consul
    //https://github.com/wintoncode/Winton.Extensions.Configuration.Consul
    //http://cecilphillip.com/using-consul-for-service-discovery-with-asp-net-core/
    public class Startup
    {
        /*
{
  "Application":{
    "Name":"Luca",
    "Age":18
  }
}
             */

        public Startup(IHostingEnvironment env)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddConsul(
                    $"{env.ApplicationName}.{env.EnvironmentName}",
                    _cancellationTokenSource.Token,
                    options =>
                    {
                        options.ConsulConfigurationOptions = cco =>
                        {
                            cco.Address = new Uri("http://consul:8500");
                        };
                        options.Optional = true;
                        options.ReloadOnChange = true;
                        options.OnLoadException = (exceptionContext) =>
                        {
                            exceptionContext.Ignore = false;
                        };
                    })
                .AddEnvironmentVariables()
                .Build();

            HostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        protected CancellationTokenSource _cancellationTokenSource;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConsulConfig>(Configuration.GetSection("ConsulConfig"));
            services.AddSingleton(Configuration);

            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                string address = Configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));

            services.AddOptions();

            services.Configure<ApplicationOptions>(Configuration.GetSection("Application"));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider provider, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            IOptions<ApplicationOptions> options = provider.GetRequiredService<IOptions<ApplicationOptions>>();
            if (options.Value.Name == null)
            {
                IConsulClient client = provider.GetRequiredService<IConsulClient>();

                client.KV.Put(new KVPair($"{env.ApplicationName}.{env.EnvironmentName}")
                {
                    Value = Encoding.UTF8.GetBytes(
@"{
  'Application':{
    'Name':'Mario',
    'Age':11
  }
}")
                }).ConfigureAwait(false).GetAwaiter().GetResult();

                appLifetime.ApplicationStopping.Register(_cancellationTokenSource.Cancel);

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseMvc();

                app.RegisterWithConsul(appLifetime, "http://webapplication1");
            }
        }
    }
}
