# Wijziging A - PDF toegestaan

## Applicatiewijziging

In de oorspronkelijke applicatie werden enkel afbeeldingsbestanden toegestaan bij een afspraakaanvraag. In deze wijziging werd de validatieregel aangepast zodat ook PDF-bestanden als geldige bijlage worden aanvaard.

De tijd voor deze applicatiewijziging werd niet meegeteld, omdat deelvraag 5 enkel de onderhoudbaarheid van de tests meet.

## xUnit-meting

Na de applicatiewijziging faalde één xUnit-test, omdat een PDF-bestand in de bestaande test nog als ongeldig werd beschouwd.

Gemeten aanpassingstijd:

- 3 minuten en 21,92 seconden
- Omgerekend: 3,37 minuten

Aangepast testbestand:

- `tests/Dokterspraktijk.Tests.xUnit/Services/AfspraakServiceTests.cs`

Resultaat:

- xUnit-tests opnieuw groen

Gemeten wijzigingsomvang:

- Gewijzigde testbestanden: 1
- Toegevoegde regels: 2
- Verwijderde regels: 1

## Reqnroll-meting

Na dezelfde applicatiewijziging faalde één Reqnroll-scenario, omdat het Gherkin-scenario PDF nog als ongeldige invoer beschouwde.

Gemeten aanpassingstijd:

- 1,42 minuten

Aangepast testbestand:

- Featurebestand voor foto toevoegen aan afspraakaanvraag

Resultaat:

- Reqnroll-tests opnieuw groen

Gemeten wijzigingsomvang:

- Gewijzigde testbestanden: 1
- Toegevoegde regels: 1
- Verwijderde regels: 1