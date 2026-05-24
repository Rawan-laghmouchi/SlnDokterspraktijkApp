# Wijziging A - PDF-bestanden worden ook toegelaten

## App-wijziging

Nieuwe businessregel:

PDF-bestanden worden voortaan ook toegestaan als bijlage bij een afspraakaanvraag.

Betrokken applicatiebestand:

- `src/Dokterspraktijk.Application/Services/Implementation/AfspraakService.cs`

De tijd voor deze applicatiewijziging wordt niet meegeteld in de onderhoudbaarheidsmeting, omdat deelvraag 5 de onderhoudbaarheid van de tests onderzoekt.

## Testresultaat direct na applicatiewijziging

Commando:

```bash
dotnet test
```

Resultaat:

- Totaal aantal tests: 54
- Geslaagd: 52
- Gefaald: 2
- Overgeslagen: 0

## Gefaalde xUnit-test

- Testklasse: `Dokterspraktijk.Tests.xUnit.Services.AfspraakServiceTests`
- Testmethode: `ValideerBestandVoorAfspraakaanvraag_OngeldigeExtensie_GeeftMisluktResultaat`
- Testdata: `bestandsnaam: "document.pdf"`
- Foutmelding: `Assert.False() Failure`
- Verwacht resultaat: `False`
- Werkelijk resultaat: `True`
- Bestand: `tests/Dokterspraktijk.Tests.xUnit/Services/AfspraakServiceTests.cs`
- Regel: 534

Interpretatie:

De xUnit-test verwachtte nog dat een PDF-bestand geweigerd werd. Door de nieuwe applicatieregel wordt een PDF-bestand nu wel geaccepteerd. Daardoor faalt deze test terecht en moet de xUnit-test aangepast worden aan de nieuwe businessregel.

## Gefaalde Reqnroll-scenario-uitvoering

- Feature: `Foto_toevoegen_aan_afspraakaanvraag.feature`
- Scenario: `Een bestand wordt gevalideerd bij het toevoegen aan een afspraakaanvraag`
- Example: `bestandstype: "PDF-bestand", resultaat: "geweigerd"`
- Foutmelding: `Assert.False() Failure`
- Verwacht resultaat: `False`
- Werkelijk resultaat: `True`
- Stepdefinition: `FotoToevoegenAanAfspraakaanvraagStepDefinitions.ThenWordtHetBestandResultaat`
- Bestand: `tests/Dokterspraktijk.Tests.BDD/StepDefinitions/FotoToevoegenAanAfspraakaanvraagStepDefinitions.cs`
- Regel: 115
- Featurebestand: `tests/Dokterspraktijk.Tests.BDD/Features/Foto_toevoegen_aan_afspraakaanvraag.feature`
- Regel: 11

Interpretatie:

De Reqnroll-scenario-uitvoering verwachtte nog dat een PDF-bestand geweigerd werd. Door de nieuwe applicatieregel wordt het PDF-bestand nu geaccepteerd. Daardoor faalt deze Reqnroll-scenario-uitvoering terecht en moet de Examples-tabel in het featurebestand aangepast worden.

## Conclusie na de app-wijziging

Na het aanpassen van de applicatielogica faalden twee tests: één xUnit-test en één Reqnroll-scenario-uitvoering. Beide fouten verwijzen naar dezelfde gewijzigde businessregel, namelijk dat PDF-bestanden niet langer geweigerd maar geaccepteerd worden. Dit bevestigt dat beide testbenaderingen deze businessregel afdekken en dat beide testsets onderhoud nodig hebben na deze wijziging.

## Onderhoudsmeting xUnit

De xUnit-testaanpassing duurde 3 minuten en 21,92 seconden. Omgerekend is dit 3,37 minuten.

Aangepast testbestand:

- `tests/Dokterspraktijk.Tests.xUnit/Services/AfspraakServiceTests.cs`

Na de aanpassing werden de xUnit-tests opnieuw uitgevoerd met:

```bash
dotnet test .\tests\Dokterspraktijk.Tests.xUnit\Dokterspraktijk.Tests.xUnit.csproj
```

Resultaat:

- De xUnit-tests werden opnieuw groen uitgevoerd.

## Git-diff xUnit-testaanpassing

Commando:

```bash
git diff --numstat wijziging-a-app-pdf-toegestaan..HEAD -- tests/Dokterspraktijk.Tests.xUnit
```

Output:

```text
2       1       tests/Dokterspraktijk.Tests.xUnit/Services/AfspraakServiceTests.cs
```

Interpretatie:

Voor de xUnit-aanpassing werd één testbestand gewijzigd. Daarbij werden twee regels toegevoegd en één regel verwijderd.

## Onderhoudsmeting Reqnroll

De Reqnroll-testaanpassing duurde 1 minuut en 25,24 seconden. Omgerekend is dit 1,42 minuten.

Aangepast testbestand:

- `tests/Dokterspraktijk.Tests.BDD/Features/Foto_toevoegen_aan_afspraakaanvraag.feature`

Na de aanpassing werden de Reqnroll-tests opnieuw uitgevoerd met:

```bash
dotnet test .\tests\Dokterspraktijk.Tests.BDD\Dokterspraktijk.Tests.BDD.csproj
```

Resultaat:

- De Reqnroll-tests werden opnieuw groen uitgevoerd.

## Git-diff Reqnroll-testaanpassing

Commando:

```bash
git diff --numstat wijziging-a-app-pdf-toegestaan..HEAD -- tests/Dokterspraktijk.Tests.BDD
```

Output:

```text
1       1       tests/Dokterspraktijk.Tests.BDD/Features/Foto_toevoegen_aan_afspraakaanvraag.feature
```

Interpretatie:

Voor de Reqnroll-aanpassing werd één featurebestand gewijzigd. Daarbij werd één regel toegevoegd en één regel verwijderd.