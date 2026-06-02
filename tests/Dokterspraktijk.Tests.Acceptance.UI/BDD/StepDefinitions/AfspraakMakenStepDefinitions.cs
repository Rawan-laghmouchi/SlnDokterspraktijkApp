using Dokterspraktijk.Tests.Acceptance.UI.BDD.Support;
using Dokterspraktijk.Tests.Acceptance.UI.Shared.Pages;
using Reqnroll;
using Xunit;

namespace Dokterspraktijk.Tests.Acceptance.UI.BDD.StepDefinitions
{
    [Binding]
    public class AfspraakMakenStepDefinitions
    {
        private UiTestContext _context;
        private AfspraakMakenPagina _afspraakMakenPagina;

        public AfspraakMakenStepDefinitions(UiTestContext context)
        {
            _context = context;

            _afspraakMakenPagina = new AfspraakMakenPagina(_context.Page!);
        }

        [Given("de patiënt bevindt zich op de pagina om een afspraak te maken")]
        public async Task GivenDePatientBevindtZichOpDePaginaOmEenAfspraakTeMaken()
        {
            await _afspraakMakenPagina.OpenAsync(_context.BasisUrl);
        }

        [When("de patiënt de volgende afspraakgegevens kiest")]
        public async Task WhenDePatientDeVolgendeAfspraakgegevensKiest(Table table)
        {
            var rij = table.Rows[0];

            string dokterNaam = rij["dokter"];
            string datum = rij["datum"];
            string tijdslot = rij["tijdslot"];
            string categorie = rij["categorie"];

            await _afspraakMakenPagina.KiesDokterAsync(dokterNaam);
            await _afspraakMakenPagina.KiesDatumAsync(datum);
            await _afspraakMakenPagina.BekijkBeschikbaarhedenAsync();
            await _afspraakMakenPagina.KiesTijdslotAsync(tijdslot);
            await _afspraakMakenPagina.KiesAfspraakcategorieAsync(categorie);
        }

        [When("de patiënt de volgende afspraakgegevens kiest zonder tijdslot")]
        public async Task WhenDePatientDeVolgendeAfspraakgegevensKiestZonderTijdslot(Table table)
        {
            var rij = table.Rows[0];

            string dokterNaam = rij["dokter"];
            string datum = rij["datum"];
            string categorie = rij["categorie"];

            await _afspraakMakenPagina.KiesDokterAsync(dokterNaam);
            await _afspraakMakenPagina.KiesDatumAsync(datum);
            await _afspraakMakenPagina.BekijkBeschikbaarhedenAsync();
            await _afspraakMakenPagina.KiesAfspraakcategorieAsync(categorie);
        }

        [When("de patiënt de volgende patiëntgegevens invult")]
        public async Task WhenDePatientDeVolgendePatientgegevensInvult(Table table)
        {
            var rij = table.Rows[0];

            string voornaam = rij["voornaam"];
            string achternaam = rij["achternaam"];
            string email = rij["email"];
            string telefoonnummer = rij["telefoonnummer"];
            string rijksregisternummer = rij["rijksregisternummer"];

            await _afspraakMakenPagina.VulPatientgegevensInAsync(
                voornaam,
                achternaam,
                email,
                telefoonnummer,
                rijksregisternummer);
        }

        [When("de patiënt de afspraak bevestigt")]
        public async Task WhenDePatientDeAfspraakBevestigt()
        {
            await _afspraakMakenPagina.BevestigAfspraakAsync();
        }

        [Then("ziet de patiënt een bevestigingsmelding")]
        public async Task ThenZietDePatientEenBevestigingsmelding()
        {
            bool isZichtbaar = await _afspraakMakenPagina.IsBevestigingsmeldingZichtbaarAsync();

            Assert.True(isZichtbaar, "De bevestigingsmelding werd niet gevonden.");
        }

        [Then("blijft de patiënt op de afspraakpagina")]
        public void ThenBlijftDePatientOpDeAfspraakpagina()
        {
            string huidigeUrl = _afspraakMakenPagina.GeefHuidigeUrl();

            Assert.Contains("/Afspraak/Create", huidigeUrl);
        }

        [Then("ziet de patiënt een foutmelding voor het ontbrekende tijdslot")]
        public async Task ThenZietDePatientEenFoutmeldingVoorHetOntbrekendeTijdslot()
        {
            bool isZichtbaar = await _afspraakMakenPagina.IsTijdslotFoutmeldingZichtbaarAsync();

            Assert.True(isZichtbaar, "De foutmelding voor het ontbrekende tijdslot werd niet gevonden.");
        }
    }
}