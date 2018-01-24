using AsIKnow.DependencyHelpers;
using AsIKnow.XUnitExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XTest.Selenium
{
    public interface Selenium
    { }
    public class SeleniumFixture : DockerEnvironmentsBaseFixture<Selenium>
    {
        protected override void ConfigureServices(ServiceCollection sc)
        {
            sc.AddLogging(builder => builder.AddDebug());
        }

        public void Configure()
        {
            WaitForDependencies(builder =>
            {
                builder.AddDependencyCheck(
                    new TcpConnectionDependencyCheck(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4444), "selenium hub", TimeSpan.FromSeconds(Options.DependencyCheckOptions.CheckTimeout)));
                builder.AddDependencyCheck(
                    new CustomDependencyCheck(() => 
                    {
                        try
                        {
                            using (new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new FirefoxOptions()))
                            {

                            }
                            return Task.FromResult(true);
                        }
                        catch { return Task.FromResult(false); }
                    }, "selenium hub firefox", TimeSpan.FromSeconds(Options.DependencyCheckOptions.CheckTimeout)));
                builder.AddDependencyCheck(
                    new CustomDependencyCheck(() =>
                    {
                        try
                        {
                            using (new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions()))
                            {

                            }
                            return Task.FromResult(true);
                        }
                        catch { return Task.FromResult(false); }
                    }, "selenium hub chrome", TimeSpan.FromSeconds(Options.DependencyCheckOptions.CheckTimeout)));
            });
        }
    }
}
