# Git-branches als bewijs van de onderhoudbaarheidsmeting

Voor deelvraag 5 werd per applicatiewijziging gewerkt met aparte Git-branches. Daardoor is traceerbaar welke wijziging eerst in de applicatielogica werd aangebracht en welke aanpassingen daarna afzonderlijk in de xUnit-tests en Reqnroll-tests gebeurden.

## Wijziging A – PDF toegestaan

| Stap                 | Branch                                  | Doel                                                             |
|----------------------|-----------------------------------------|------------------------------------------------------------------|
| Applicatiewijziging  | `wijziging-a-app-pdf-toegestaan`        | Applicatielogica aanpassen zodat PDF-bestanden toegestaan worden |
| xUnit-aanpassing     | `wijziging-a-xunit-pdf-toegestaan`      | xUnit-tests aanpassen aan de nieuwe PDF-regel                    |
| Reqnroll-aanpassing  | `wijziging-a-reqnroll-pdf-toegestaan`   | BDD-scenario of Gherkin-test aanpassen aan de nieuwe PDF-regel   |

## Wijziging B – Doktersattest één keer downloaden

| Stap                 | Branch                                               | Doel                                                                                      |
|----------------------|------------------------------------------------------|-------------------------------------------------------------------------------------------|
| Applicatiewijziging  | `wijziging-b-app-attest-een-keer-downloaden`         | Applicatielogica aanpassen zodat een doktersattest slechts één keer gedownload mag worden |
| xUnit-aanpassing     | `wijziging-b-xunit-attest-een-keer-downloaden`       | xUnit-tests uitbreiden met controle op een tweede downloadpoging                          |
| Reqnroll-aanpassing  | `wijziging-b-reqnroll-attest-een-keer-downloaden`    | BDD-scenario en stepdefinitions uitbreiden met controle op een tweede downloadpoging      |

## Wijziging C – Alleen geplande afspraken annuleren

| Stap                 | Branch                                             | Doel                                                                                |
|----------------------|----------------------------------------------------|-------------------------------------------------------------------------------------|
| Applicatiewijziging  | `wijziging-c-app-annulatie-alleen-gepland`         | Applicatielogica aanpassen zodat alleen geplande afspraken geannuleerd mogen worden |
| xUnit-aanpassing     | `wijziging-c-xunit-annulatie-alleen-gepland`       | xUnit-tests aanpassen aan de nieuwe annulatielogica                                 |
| Reqnroll-aanpassing  | `wijziging-c-reqnroll-annulatie-alleen-gepland`    | BDD-scenario en stepdefinitions aanpassen aan de nieuwe annulatielogica             |

## Toelichting

De tijd voor de applicatiewijziging werd niet meegeteld in de onderhoudbaarheidsmeting. De stopwatch werd pas gestart bij het aanpassen van de testcode. Voor xUnit gebeurde dit in de xUnit-branches. Voor Reqnroll gebeurde dit in de Reqnroll-branches. Hierdoor blijven de metingen per testbenadering gescheiden en controleerbaar.