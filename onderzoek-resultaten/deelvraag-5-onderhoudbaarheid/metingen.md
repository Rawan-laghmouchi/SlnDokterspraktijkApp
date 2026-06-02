# Metingen deelvraag 5 – Onderhoudbaarheid

Deze metingen horen bij deelvraag 5 van de bachelorproef. De onderhoudbaarheid werd gemeten aan de hand van aanpassingstijd, aantal gewijzigde testbestanden, gewijzigde regels code en stabiliteit over tien CI-runs.

De tijd voor het aanpassen van de applicatielogica werd niet meegeteld. De stopwatch werd pas gestart wanneer de testaanpassing begon en werd gestopt wanneer het betreffende testproject opnieuw groen was.

## Meetresultaten per wijziging

| Wijziging                                | Testbenadering | Falende tests na applicatiewijziging | Aanpassingstijd | Gewijzigde testbestanden | Toegevoegde regels | Verwijderde regels | Resultaat |
|------------------------------------------|----------------|--------------------------------------|-----------------|--------------------------|--------------------|--------------------|-----------|
| A – PDF toegestaan                       | xUnit          | 1                                    | 3,37 min.       | 1                        | 2                  | 1                  | Groen     |
| A – PDF toegestaan                       | Reqnroll       | 1                                    | 1,42 min.       | 1                        | 1                  | 1                  | Groen     |
| B – Attest één keer downloaden           | xUnit          | 0                                    | 14,03 min.      | 1                        | 47                 | 0                  | Groen     |
| B – Attest één keer downloaden           | Reqnroll       | 0                                    | 7,40 min.       | 2                        | 20                 | 1                  | Groen     |
| C – Alleen geplande afspraak annuleren   | xUnit          | 1                                    | 11,18 min.      | 1                        | 43                 | 1                  | Groen     |
| C – Alleen geplande afspraak annuleren   | Reqnroll       | 1                                    | 11,03 min.      | 2                        | 62                 | 0                  | Groen     |

## Samenvatting per testbenadering

| Testbenadering | Totale aanpassingstijd | Gemiddelde aanpassingstijd | Gewijzigde testbestanden | Toegevoegde regels | Verwijderde regels |
|----------------|------------------------|----------------------------|--------------------------|--------------------|--------------------|
| xUnit          | 28,58 min.             | 9,53 min.                  | 3                        | 92                 | 2                  |
| Reqnroll       | 19,85 min.             | 6,62 min.                  | 5                        | 83                 | 2                  |