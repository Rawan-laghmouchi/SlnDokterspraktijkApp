# Wijziging C - Een geannuleerde afspraak kan niet opnieuw geannuleerd worden

## App-wijziging

Nieuwe businessregel:

Alleen een geplande afspraak kan geannuleerd worden. Een afspraak die al geannuleerd of afgerond is, 
kan dus niet opnieuw geannuleerd worden.

Betrokken applicatiebestanden:

- `src/Dokterspraktijk.Domain/Entities/Afspraak.cs`
- `src/Dokterspraktijk.Application/Services/Implementation/AfspraakService.cs`

De tijd voor deze applicatiewijziging wordt niet meegeteld in de onderhoudbaarheidsmeting, omdat deelvraag 5 de 
onderhoudbaarheid van de tests onderzoekt.

## Testresultaat direct na applicatiewijziging

Na de applicatiewijziging faalde minstens één bestaande xUnit-test, omdat de verwachte foutmelding nog verwees naar 
de oude businessregel.

Gefaalde xUnit-test:

- Testklasse: `Dokterspraktijk.Tests.xUnit.Services.AfspraakServiceTests`
- Testmethode: `AnnuleerAfspraak_AfgerondeAfspraak_GeeftMisluktResultaat`
- Oorzaak: de test verwachtte nog de oude melding `Een afgeronde afspraak kan niet geannuleerd worden.`
- Nieuwe melding: `Alleen een geplande afspraak kan geannuleerd worden.`

Daarnaast werd een nieuwe xUnit-test toegevoegd om expliciet te controleren dat een geannuleerde afspraak niet 
opnieuw geannuleerd kan worden.

## Onderhoudsmeting xUnit

De xUnit-testaanpassing duurde 11 minuten en 10,80 seconden. Omgerekend is dit 11,18 minuten.

Aangepast testbestand:

- `tests/Dokterspraktijk.Tests.xUnit/Services/AfspraakServiceTests.cs`

Tijdens de xUnit-aanpassing werd de bestaande test voor een afgeronde afspraak aangepast aan de nieuwe foutmelding. 
Daarnaast werd een nieuwe test toegevoegd voor het scenario waarbij een reeds geannuleerde afspraak opnieuw geannuleerd 
wordt. Tijdens het aanpassen werd de variabele `tweedeAnnulatie` eerst vergeten te declareren. Deze fout werd gecorrigeerd
voordat de meting werd afgerond.

Na de aanpassing werden de xUnit-tests opnieuw uitgevoerd met:

```bash
dotnet test .\tests\Dokterspraktijk.Tests.xUnit\Dokterspraktijk.Tests.xUnit.csproj
```

Resultaat:

- De xUnit-tests werden opnieuw groen uitgevoerd.

## Git-diff xUnit-testaanpassing

Commando:

```bash
git diff --numstat wijziging-c-app-annulatie-alleen-gepland..HEAD -- tests/Dokterspraktijk.Tests.xUnit
```

Output:

```text
43      1       tests/Dokterspraktijk.Tests.xUnit/Services/AfspraakServiceTests.cs
```

Interpretatie:

Voor de xUnit-aanpassing werd één testbestand gewijzigd. Daarbij werden 43 regels toegevoegd en één regel verwijderd. 
De wijziging bestond uit het aanpassen van een bestaande assert naar de nieuwe foutmelding en het toevoegen van een 
nieuwe testmethode voor het opnieuw annuleren van een reeds geannuleerde afspraak.

## Onderhoudsmeting Reqnroll

De Reqnroll-testaanpassing duurde 11 minuten en 01,97 seconden. Omgerekend is dit 11,03 minuten.

Aangepaste testbestanden:

- `tests/Dokterspraktijk.Tests.BDD/Features/Afspraak_annuleren.feature`
- `tests/Dokterspraktijk.Tests.BDD/StepDefinitions/AfspraakAnnulerenStepDefinitions.cs`

Tijdens de Reqnroll-aanpassing werd een nieuw scenario toegevoegd aan het featurebestand. Daarnaast werd een extra Given-step 
toegevoegd in de stepdefinitionklasse om een reeds geannuleerde afspraak als startsituatie klaar te zetten.

Na de aanpassing werden de Reqnroll-tests opnieuw uitgevoerd met:

```bash
dotnet test .\tests\Dokterspraktijk.Tests.BDD\Dokterspraktijk.Tests.BDD.csproj
```

Resultaat:

- De Reqnroll-tests werden opnieuw groen uitgevoerd.

## Git-diff Reqnroll-testaanpassing

Commando:

```bash
git diff --numstat wijziging-c-app-annulatie-alleen-gepland..HEAD -- tests/Dokterspraktijk.Tests.BDD
```

Output:

```text
5       0       tests/Dokterspraktijk.Tests.BDD/Features/Afspraak_annuleren.feature
57      0       tests/Dokterspraktijk.Tests.BDD/StepDefinitions/AfspraakAnnulerenStepDefinitions.cs
```

Interpretatie:

Voor de Reqnroll-aanpassing werden twee testbestanden gewijzigd. In totaal werden 62 regels toegevoegd en geen regels verwijderd. 
De wijziging bestond uit het toevoegen van een nieuw Gherkin-scenario en een extra stepdefinition voor het scenario waarin een reeds 
geannuleerde afspraak opnieuw geannuleerd wordt.