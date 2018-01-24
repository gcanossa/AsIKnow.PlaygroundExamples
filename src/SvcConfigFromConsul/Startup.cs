using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Winton.Extensions.Configuration.Consul;

namespace SvcConfigFromConsul
{
    public class Startup
    {
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
                            cco.Address = new Uri("http://localhost:8500");
                        };
                        options.Optional = false;
                        options.ReloadOnChange = true;
                        options.OnLoadException = (exceptionContext) =>
                        {
                            exceptionContext.Ignore = true;
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
            services.AddOptions();

            services.Configure<ApplicationOptions>(Configuration.GetSection("Application"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStopping.Register(_cancellationTokenSource.Cancel);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
