# Wijziging C - Alleen geplande afspraken annuleren

## Applicatiewijziging

In deze wijziging werd de annulatielogica aangepast. Alleen afspraken met de status `Gepland` mogen nog geannuleerd worden. Afspraken die al `Geannuleerd` of `Afgerond` zijn, mogen niet opnieuw geannuleerd worden.

De tijd voor deze applicatiewijziging werd niet meegeteld, omdat deelvraag 5 enkel de onderhoudbaarheid van de tests meet.

## xUnit-meting

Na de applicatiewijziging faalde één xUnit-test. De test moest worden aangepast zodat het nieuwe gedrag rond afspraakstatussen correct werd gecontroleerd.

Gemeten aanpassingstijd:

- 11,18 minuten

Aangepast testbestand:

- xUnit-testbestand voor afspraken annuleren

Resultaat:

- xUnit-tests opnieuw groen

Gemeten wijzigingsomvang:

- Gewijzigde testbestanden: 1
- Toegevoegde regels: 43
- Verwijderde regels: 1

## Reqnroll-meting

Na dezelfde applicatiewijziging faalde één Reqnroll-scenario. Om de nieuwe businessregel duidelijk te beschrijven, werd het Gherkin-scenario aangepast en werd bijkomende stepdefinitioncode voorzien.

Gemeten aanpassingstijd:

- 11,03 minuten

Aangepaste testbestanden:

- Featurebestand voor afspraak annuleren
- Stepdefinitionklasse voor afspraak annuleren

Resultaat:

- Reqnroll-tests opnieuw groen

Gemeten wijzigingsomvang:

- Gewijzigde testbestanden: 2
- Toegevoegde regels: 62
- Verwijderde regels: 0