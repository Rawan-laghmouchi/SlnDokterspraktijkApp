# Metingen onderhoudbaarheid

| Wijziging                              | Testbenadering | Falende tests na app-wijziging | Starttijd | Eindtijd | Minuten | Gewijzigde testbestanden | Toegevoegde regels | Verwijderde regels | Resultaat |
|----------------------------------------|----------------|-------------------------------:|-----------|----------|--------:|-------------------------:|-------------------:|-------------------:|-----------|
| A - PDF toegestaan                     | xUnit          |                              1 | ...       | ...      |    3,37 |                        1 |                  2 |                  1 | Groen     |
| A - PDF toegestaan                     | Reqnroll       |                              1 | ...       | ...      |    1,42 |                        1 |                  1 |                  1 | Groen     |
| B - Attest één keer downloaden         | xUnit          |                              0 | ...       | ...      |   14,03 |                        1 |                 47 |                  0 | Groen     |
| B - Attest één keer downloaden         | Reqnroll       |                              0 | ...       | ...      |    7,40 |                        2 |                 20 |                  1 | Groen     |
| C - Alleen geplande afspraak annuleren | xUnit          |                              1 | ...       | ...      |   11,18 |                        1 |                 43 |                  1 | Groen     |
| C - Alleen geplande afspraak annuleren | Reqnroll       |                              1 | ...       | ...      |   11,03 |                        2 |                 62 |                  0 | Groen     |