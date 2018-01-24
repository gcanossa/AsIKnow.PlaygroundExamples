using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XTest.Selenium
{
    [CollectionDefinition("Selenium collection")]
    public class SeleniumCollection : ICollectionFixture<SeleniumFixture>
    {
    }
}
