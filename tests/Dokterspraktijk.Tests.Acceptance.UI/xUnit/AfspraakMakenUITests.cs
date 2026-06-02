using Dokterspraktijk.Tests.Acceptance.UI.Shared.Pages;
using Microsoft.Playwright;
using Xunit;

namespace Dokterspraktijk.Tests.Acceptance.UI.XUnit
{
    public class AfspraakMakenUITests
    {
        private const string BasisUrl = "http://localhost:5111";

        [Fact]
        public async Task PatientKanAfspraakMakenViaDeGebruikersinterface()
        {
            IPlaywright playwright = await Playwright.CreateAsync();

            IBrowser browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false
                });

            IBrowserContext browserContext = await browser.NewContextAsync(
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

            IPage page = await browserContext.NewPageAsync();

            try
            {
                AfspraakMakenPagina afspraakMakenPagina = new AfspraakMakenPagina(page);

                await afspraakMakenPagina.OpenAsync(BasisUrl);

                await afspraakMakenPagina.KiesDokterAsync("Timmermans");
                await afspraakMakenPagina.KiesDatumAsync("2026-06-15");
                await afspraakMakenPagina.BekijkBeschikbaarhedenAsync();

                await afspraakMakenPagina.KiesTijdslotAsync("10:30");

                await afspraakMakenPagina.VulPatientgegevensInAsync(
                    "Hans",
                    "Vandenbogaerde",
                    "hans.vandenbogaerde.ui.xunit@gmail.be",
                    "+32 470 12 34 56",
                    "00.01.01-001.01");

                await afspraakMakenPagina.KiesAfspraakcategorieAsync("Consultatie");
                await afspraakMakenPagina.BevestigAfspraakAsync();

                bool bevestigingsmeldingIsZichtbaar = await afspraakMakenPagina.IsBevestigingsmeldingZichtbaarAsync();

                Assert.True(bevestigingsmeldingIsZichtbaar, "De bevestigingsmelding werd niet gevonden.");
            }
            finally
            {
                await browserContext.CloseAsync();
                await browser.CloseAsync();
                playwright.Dispose();
            }
        }

        [Fact]
        public async Task AfspraakMakenZonderTijdslotToontFoutmelding()
        {
            IPlaywright playwright = await Playwright.CreateAsync();

            IBrowser browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false
                });

            IBrowserContext browserContext = await browser.NewContextAsync(
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

            IPage page = await browserContext.NewPageAsync();

            try
            {
                AfspraakMakenPagina afspraakMakenPagina = new AfspraakMakenPagina(page);

                await afspraakMakenPagina.OpenAsync(BasisUrl);

                await afspraakMakenPagina.KiesDokterAsync("Timmermans");
                await afspraakMakenPagina.KiesDatumAsync("2026-06-15");
                await afspraakMakenPagina.BekijkBeschikbaarhedenAsync();

                await afspraakMakenPagina.VulPatientgegevensInAsync(
                    "Hans",
                    "Vandenbogaerde",
                    "hans.vandenbogaerde.ui.zondertijdslot.xunit@gmail.be",
                    "+32 470 12 34 56",
                    "00.01.01-001.01");

                await afspraakMakenPagina.KiesAfspraakcategorieAsync("Consultatie");
                await afspraakMakenPagina.BevestigAfspraakAsync();

                string huidigeUrl = afspraakMakenPagina.GeefHuidigeUrl();
                bool foutmeldingIsZichtbaar = await afspraakMakenPagina.IsTijdslotFoutmeldingZichtbaarAsync();

                Assert.Contains("/Afspraak/Create", huidigeUrl);
                Assert.True(foutmeldingIsZichtbaar, "De foutmelding voor het ontbrekende tijdslot werd niet gevonden.");
            }
            finally
            {
                await browserContext.CloseAsync();
                await browser.CloseAsync();
                playwright.Dispose();
            }
        }
    }
}