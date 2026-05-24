## Opmerking bij de xUnit-meting

Tijdens de xUnit-aanpassing trad eerst een compilefout op door een typfout in de naam van de variabele `tweedeDownload`. 
Daarna faalde tijdelijk nog een test uit `AfspraakServiceTests`, omdat in deze branch per ongeluk een eerdere PDF-testwijziging uit wijziging A aanwezig was. 
Deze verdwaalde wijziging werd teruggezet naar de baseline, omdat wijziging B uitsluitend betrekking heeft op het eenmalig downloaden van een doktersattest.

De stopwatch werd pas gestopt nadat het xUnit-testproject opnieuw groen was. Deze tijd werd behouden als gemeten onderhoudstijd, 
omdat het corrigeren van testcode en het herstellen van de testbranch deel uitmaakt van de praktische onderhoudsinspanning.

# Wijziging B - Doktersattest mag maar één keer gedownload worden

## App-wijziging

Nieuwe businessregel:

Een doktersattest mag voortaan maar één keer gedownload worden.

Betrokken applicatiebestand:

- `src/Dokterspraktijk.Application/Services/Implementation/DoktersattestService.cs`

De tijd voor deze applicatiewijziging wordt niet meegeteld in de onderhoudbaarheidsmeting, omdat deelvraag 5 de onderhoudbaarheid van de tests onderzoekt.

## Testresultaat direct na applicatiewijziging

Na de applicatiewijziging faalden er geen bestaande xUnit-tests voor deze nieuwe businessregel. Daarom wordt deze wijziging beschouwd als een uitbreiding 
van de testset, en niet als een zuivere herstelling van bestaande falende tests.

## Onderhoudsmeting xUnit

De xUnit-testaanpassing duurde 14 minuten en 01,58 seconden. Omgerekend is dit 14,03 minuten.

Aangepast testbestand:

- `tests/Dokterspraktijk.Tests.xUnit/Services/DoktersattestServiceTests.cs`

Tijdens de xUnit-aanpassing trad eerst een compilefout op door een typfout in de naam van de variabele `tweedeDownload`. 
Daarna faalde tijdelijk nog een test uit `AfspraakServiceTests`, omdat in deze branch per ongeluk een eerdere PDF-testwijziging uit wijziging 
A aanwezig was. Deze verdwaalde wijziging werd teruggezet naar de baseline, omdat wijziging B uitsluitend betrekking heeft op het eenmalig downloaden 
van een doktersattest.

De stopwatch werd pas gestopt nadat het xUnit-testproject opnieuw groen was. Deze tijd werd behouden als gemeten onderhoudstijd, omdat het corrigeren van 
testcode en het herstellen van de testbranch deel uitmaakt van de praktische onderhoudsinspanning.

Na de aanpassing werden de xUnit-tests opnieuw uitgevoerd met:

```bash
dotnet test .\tests\Dokterspraktijk.Tests.xUnit\Dokterspraktijk.Tests.xUnit.csproj
```

Resultaat:

- De xUnit-tests werden opnieuw groen uitgevoerd.

## Git-diff xUnit-testaanpassing

Commando vóór commit:

```bash
git diff --numstat -- tests/Dokterspraktijk.Tests.xUnit/Services/DoktersattestServiceTests.cs
```

Output:

```text
47      0       tests/Dokterspraktijk.Tests.xUnit/Services/DoktersattestServiceTests.cs
```

Commando na commit:

```bash
git diff --numstat wijziging-b-app-attest-een-keer-downloaden..HEAD -- tests/Dokterspraktijk.Tests.xUnit
```

Output:

```text
47      0       tests/Dokterspraktijk.Tests.xUnit/Services/DoktersattestServiceTests.cs
```

Interpretatie:

Voor de xUnit-aanpassing werd één testbestand gewijzigd. Daarbij werden 47 regels toegevoegd en geen regels verwijderd. 
Deze wijziging bestond uit het toevoegen van een nieuwe testmethode voor de nieuwe businessregel dat een doktersattest maar één keer gedownload mag worden.

# Metingen onderhoudbaarheid

| Wijziging | Testbenadering | Falende tests na app-wijziging | Starttijd | Eindtijd | Minuten | Gewijzigde testbestanden | Toegevoegde regels | Verwijderde regels | Resultaat |
|---|---|---:|---|---|---:|---:|---:|---:|---|
| A - PDF toegestaan | xUnit | 1 | niet afzonderlijk genoteerd | niet afzonderlijk genoteerd | 3,37 | 1 | 2 | 1 | Groen |
| A - PDF toegestaan | Reqnroll | 1 | niet afzonderlijk genoteerd | niet afzonderlijk genoteerd | 1,42 | 1 | 1 | 1 | Groen |
| B - Attest één keer downloaden | xUnit | 0 | niet afzonderlijk genoteerd | niet afzonderlijk genoteerd | 14,03 | 1 | 47 | 0 | Groen |
| B - Attest één keer downloaden | Reqnroll | 0 | niet afzonderlijk genoteerd | niet afzonderlijk genoteerd | 7,40 | 2 | 20 | 1 | Groen |