using System;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using AsIKnow.XUnitExtensions;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.IO;

namespace XTest.Selenium
{
    [TestCaseOrderer(Constants.PriorityOrdererTypeName, Constants.PriorityOrdererTypeAssemblyName)]
    [Collection("Selenium collection")]
    public class UnitTest1
    {
        [Trait("Category", "Selenium")]
        [Fact(DisplayName = nameof(SeleniumDocumentationExample))]
        [TestPriority(0)]
        public void SeleniumDocumentationExample()
        {
            using (IWebDriver driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new FirefoxOptions()))
            {
                //Notice navigation is slightly different than the Java version
                //This is because 'get' is a keyword in C#
                driver.Navigate().GoToUrl("http://www.google.com/");

                // Find the text input element by its name
                IWebElement query = driver.FindElement(By.Name("q"));

                // Enter something to search for
                query.SendKeys("Cheese");

                // Now submit the form. WebDriver will find the form for us from the element
                query.Submit();

                // Google's search is rendered dynamically with JavaScript.
                // Wait for the page to load, timeout after 10 seconds
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.Title.StartsWith("cheese", StringComparison.OrdinalIgnoreCase));

                // Should see: "Cheese - Google Search" (for an English locale)
                Console.WriteLine("Page title is: " + driver.Title);

                string ttt = driver.Title;

                Assert.NotNull(driver.Title);
            }
        }

        [Trait("Category", "Selenium")]
        [Fact(DisplayName = nameof(TestOnAliasOk))]
        [TestPriority(0)]
        public void TestOnAliasOk()
        {
            using (RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), new ChromeOptions()))
            {
                //naviga all'url
                driver.Navigate().GoToUrl("http://www.aliaslab.net/");
                
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                //aspetta fino a che compare il bottone blu con scritto "Scopri di più"
                wait.Until(d => driver.FindElements(By.LinkText("Scopri di più")).Any());

                var link = driver.FindElement(By.LinkText("Scopri di più"));

                //controlla l'href
                Assert.EndsWith("page_id=1496", link.GetAttribute("href"));
                
                Actions actions = new Actions(driver);

                //fa click sul tasto
                actions.MoveToElement(link).Click().Perform();

                //naviga alla destinazione del link, non ho capito perché non lo fa già al click
                //driver.Navigate().GoToUrl(link.GetAttribute("href"));

                //aspetta fino a che non cambia l'indirizzo
                wait.Until(d => d.Url == "http://www.aliaslab.net/index.php/our-team-business/");

                //aspetta fino a che non compare la scritta a sinistra/metà
                wait.Until(d => d.FindElements(By.TagName("strong")).Any(p=>p.Text == "Valorizzare ogni minimo dettaglio dell’organizzazione per ottimizzare le soluzioni, innovare il mercato e inventare il futuro."));
                
                //fa uno screenshot e lo salva (./ corrsiponde a dove sta girando il codice, quindi /bin/Debug/...)
                driver.GetScreenshot().SaveAsFile("./screenshot0.png", ScreenshotImageFormat.Png);
                //controlla che sia invisibile il box "rimani in contatto che compare a destra se scrolli"
                Assert.StartsWith("-", driver.FindElements(By.TagName("div")).First(p => p.GetAttribute("class").Contains("mailmunch-scrollbox")).GetCssValue("bottom"));
                
                //scrolla di 500px in basso
                driver.ExecuteScript("window.scrollBy(0,700)");
                //fa altro screenshot
                driver.GetScreenshot().SaveAsFile("./screenshot1.png", ScreenshotImageFormat.Png);

                //aspetta che sia visibile il box a destra
                wait.Until(d => d.FindElements(By.TagName("div")).First(p=>p.GetAttribute("class").Contains("mailmunch-scrollbox")).GetCssValue("bottom")=="0px");
            }
        }
    }
}
