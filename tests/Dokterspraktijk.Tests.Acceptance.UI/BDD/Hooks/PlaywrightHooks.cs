using Dokterspraktijk.Tests.Acceptance.UI.BDD.Support;
using Microsoft.Playwright;
using Reqnroll;

namespace Dokterspraktijk.Tests.Acceptance.UI.BDD.Hooks
{
    [Binding]
    public class PlaywrightHooks
    {
        private UiTestContext _context;

        public PlaywrightHooks(UiTestContext context)
        {
            _context = context;
        }

        [BeforeScenario]
        public async Task StartBrowser()
        {
            _context.Playwright = await Playwright.CreateAsync();

            _context.Browser = await _context.Playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false
                });

            _context.BrowserContext = await _context.Browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    IgnoreHTTPSErrors = true,
                    ViewportSize = new ViewportSize
                    {
                        Width = 1280,
                        Height = 900
                    },
                    ReducedMotion = ReducedMotion.Reduce
                });

            _context.Page = await _context.BrowserContext.NewPageAsync();
        }

        [AfterScenario]
        public async Task SluitBrowser()
        {
            if (_context.BrowserContext != null)
            {
                await _context.BrowserContext.CloseAsync();
            }

            if (_context.Browser != null)
            {
                await _context.Browser.CloseAsync();
            }

            _context.Playwright?.Dispose();
        }
    }
}