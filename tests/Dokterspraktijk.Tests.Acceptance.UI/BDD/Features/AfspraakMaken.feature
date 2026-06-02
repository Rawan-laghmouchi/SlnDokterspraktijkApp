Feature: Afspraak maken

Scenario: Een patiënt maakt succesvol een afspraak bij een beschikbaar tijdslot
    Given de patiënt bevindt zich op de pagina om een afspraak te maken
    When de patiënt de volgende afspraakgegevens kiest
        | dokter     | datum      | tijdslot | categorie   |
        | Timmermans | 2026-06-15 | 09:00    | Consultatie |
    And de patiënt de volgende patiëntgegevens invult
        | voornaam | achternaam     | email                        | telefoonnummer   | rijksregisternummer |
        | Hans     | Vandenbogaerde | Hans.Vandenbogaerde@gmail.be | +32 470 12 34 56 | 00.01.01-001.01     |
    And de patiënt de afspraak bevestigt
    Then ziet de patiënt een bevestigingsmelding

Scenario: Een patiënt probeert een afspraak te maken zonder tijdslot
    Given de patiënt bevindt zich op de pagina om een afspraak te maken
    When de patiënt de volgende afspraakgegevens kiest zonder tijdslot
        | dokter     | datum      | categorie   |
        | Timmermans | 2026-06-15 | Consultatie |
    And de patiënt de volgende patiëntgegevens invult
        | voornaam | achternaam     | email                        | telefoonnummer    | rijksregisternummer |
        | Hans     | Vandenbogaerde | Hans.Vandenbogaerde@gmail.be | +32 470 12 34 56  | 00.01.01-001.01     |
    And de patiënt de afspraak bevestigt
    Then blijft de patiënt op de afspraakpagina
    And ziet de patiënt een foutmelding voor het ontbrekende tijdslot