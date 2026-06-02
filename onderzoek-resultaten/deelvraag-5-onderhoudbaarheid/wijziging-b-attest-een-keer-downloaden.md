# Wijziging B - Doktersattest één keer downloaden

## Applicatiewijziging

In deze wijziging werd een nieuwe businessregel toegevoegd: een vrijgegeven doktersattest mag slechts één keer gedownload worden. Na een eerste succesvolle download moet een tweede downloadpoging geweigerd worden.

De tijd voor deze applicatiewijziging werd niet meegeteld, omdat deelvraag 5 enkel de onderhoudbaarheid van de tests meet.

## xUnit-meting

Na de applicatiewijziging faalde er geen bestaande xUnit-test. De bestaande testset controleerde namelijk nog niet expliciet dat een doktersattest niet opnieuw gedownload mocht worden. Daarom werd deze wijziging beschouwd als een uitbreiding van de testset.

Gemeten aanpassingstijd:

- 14,03 minuten

Aangepast testbestand:

- `tests/Dokterspraktijk.Tests.xUnit/Services/DoktersattestServiceTests.cs`

Resultaat:

- xUnit-tests groen

Gemeten wijzigingsomvang:

- Gewijzigde testbestanden: 1
- Toegevoegde regels: 47
- Verwijderde regels: 0

## Opmerking bij de xUnit-meting

Tijdens de xUnit-aanpassing trad eerst een fout op door een typfout in de naam van de variabele `tweedeDownload`. Daarna faalde tijdelijk nog een test uit `AfspraakServiceTests`, omdat in deze branch per ongeluk een eerdere PDF-testwijziging uit wijziging A aanwezig was. Deze verdwaalde wijziging werd teruggezet naar de baseline, omdat wijziging B uitsluitend betrekking heeft op het eenmalig downloaden van een doktersattest.

De stopwatch werd pas gestopt nadat het xUnit-testproject opnieuw groen was. Deze tijd werd behouden als gemeten onderhoudstijd, omdat het corrigeren van testcode en het herstellen van de testbranch deel uitmaakt van de praktische onderhoudsinspanning.

## Reqnroll-meting

Na dezelfde applicatiewijziging faalde er geen bestaand Reqnroll-scenario. Ook hier moest dus een nieuw scenario worden toegevoegd om de nieuwe businessregel expliciet te controleren.

Gemeten aanpassingstijd:

- 7,40 minuten

Aangepaste testbestanden:

- Featurebestand voor doktersattest downloaden
- Stepdefinitionklasse voor doktersattest downloaden

Resultaat:

- Reqnroll-tests groen

Gemeten wijzigingsomvang:

- Gewijzigde testbestanden: 2
- Toegevoegde regels: 20
- Verwijderde regels: 1