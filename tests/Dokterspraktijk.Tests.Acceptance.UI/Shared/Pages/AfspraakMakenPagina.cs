using Dokterspraktijk.Tests.Acceptance.UI.Shared.Pages.Abstract;
using Microsoft.Playwright;

namespace Dokterspraktijk.Tests.Acceptance.UI.Shared.Pages
{
    public class AfspraakMakenPagina : PageObject
    {
        public AfspraakMakenPagina(IPage page) : base(page)
        {
        }

        public async Task OpenAsync(string basisUrl)
        {
            await Page.GotoAsync($"{basisUrl}/Afspraak/Create");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task KiesDokterAsync(string dokterNaam)
        {
            ILocator dokterInput = Page.Locator($"[data-testid='dokter-keuze'][value='{dokterNaam}']");
            string? dokterId = await dokterInput.GetAttributeAsync("id");

            if (string.IsNullOrWhiteSpace(dokterId))
            {
                throw new InvalidOperationException($"De dokter '{dokterNaam}' werd gevonden, maar heeft geen id-attribuut.");
            }

            ILocator dokterLabel = Page.Locator($"label[for='{dokterId}']");

            await dokterLabel.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task KiesDatumAsync(string datum)
        {
            ILocator datumVeld = Page.GetByTestId("datum-veld");

            await datumVeld.FillAsync(datum);

            // In de view wordt de GET-form verzonden via onchange="this.form.submit()".
            // FillAsync alleen is niet altijd genoeg om onchange te triggeren.
            await datumVeld.DispatchEventAsync("change");

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task BekijkBeschikbaarhedenAsync()
        {
            // De oude knop 'Bekijk beschikbaarheden' bestaat niet meer.
            // De beschikbaarheden worden nu automatisch opgehaald na het kiezen van dokter en datum.
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task KiesTijdslotAsync(string tijdslot)
        {
            ILocator tijdslotInput = Page.Locator($"[data-testid='tijdslot-keuze'][value='{tijdslot}']");
            string? tijdslotId = await tijdslotInput.GetAttributeAsync("id");

            if (string.IsNullOrWhiteSpace(tijdslotId))
            {
                throw new InvalidOperationException($"Het tijdslot '{tijdslot}' werd gevonden, maar heeft geen id-attribuut.");
            }

            ILocator tijdslotLabel = Page.Locator($"label[for='{tijdslotId}']");
            await tijdslotLabel.ClickAsync();
        }

        public async Task VulPatientgegevensInAsync(
            string voornaam,
            string achternaam,
            string email,
            string telefoonnummer,
            string rijksregisternummer)
        {
            await Page.GetByTestId("patient-voornaam-veld").FillAsync(voornaam);
            await Page.GetByTestId("patient-achternaam-veld").FillAsync(achternaam);
            await Page.GetByTestId("email-veld").FillAsync(email);
            await Page.GetByTestId("telefoonnummer-veld").FillAsync(telefoonnummer);
            await Page.GetByTestId("rijksregisternummer-veld").FillAsync(rijksregisternummer);
        }

        public async Task KiesAfspraakcategorieAsync(string afspraakcategorie)
        {
            await Page.GetByTestId("reden-select").SelectOptionAsync(
                new SelectOptionValue
                {
                    Label = afspraakcategorie
                });
        }

        public async Task BevestigAfspraakAsync()
        {
            await Page.GetByTestId("afspraak-bevestigen-knop").ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        public async Task<bool> IsBevestigingsmeldingZichtbaarAsync()
        {
            return await Page.GetByTestId("bevestigingsmelding").IsVisibleAsync();
        }
        public async Task<bool> IsTijdslotFoutmeldingZichtbaarAsync()
        {
            string foutmelding = await Page.GetByTestId("tijdslot-foutmelding").InnerTextAsync();

            return !string.IsNullOrWhiteSpace(foutmelding);
        }

        public string GeefHuidigeUrl()
        {
            return Page.Url;
        }
    }
}