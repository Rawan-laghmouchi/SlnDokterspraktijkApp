using Microsoft.Playwright;

namespace Dokterspraktijk.Tests.Acceptance.UI.BDD.Support
{
    public class UiTestContext
    {
        public IPlaywright? Playwright { get; set; }

        public IBrowser? Browser { get; set; }

        public IBrowserContext? BrowserContext { get; set; }

        public IPage? Page { get; set; }

        public string BasisUrl { get; } = "http://localhost:5111";
    }
}