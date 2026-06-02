Feature: Komende afspraken bekijken

Als patiënt 
wil ik een overzicht kunnen zien van mijn komende afspraken met datum, dokter en reden van afspraak
zodat ik weet wanneer en waarvoor ik ingepland ben 

@tag1
Scenario: Een patiënt ziet haar komende afspraken
	Given patiënt Rawan Laghmouchi heeft de volgende afspraken:
	| Datum      | Tijd  | Dokter     | Reden                |
    | 15-05-2026 | 10:30 | Timmermans | algemene consultatie |
    | 20-05-2026 | 09:00 | Brancaert  | huidcontrole         |
	When zij haar komende afspraken raadpleegt
	Then ziet zij de volgende afspraken:
	| Datum      | Tijd  | Dokter     | Reden                |
    | 15-05-2026 | 10:30 | Timmermans | algemene consultatie |
    | 20-05-2026 | 09:00 | Brancaert  | huidcontrole         |

Scenario: een patiënt ziet enkel haar eigen afspraken
	Given patiënt Rawan Laghmouchi heeft de volgende afspraken:
	| Datum      | Tijd  | Dokter     | Reden                |
    | 15-05-2026 | 10:30 | Timmermans | algemene consultatie |
	And patiënt Hans Vandenbogaerde heeft volgende afspraken:
	| Datum      | Tijd  | Dokter     | Reden                |
    | 20-05-2026 | 09:00 | Brancaert  | huidcontrole         |
	When patiënt Rawan Laghmouchi haar komende afspraken raadpleegt
	Then ziet zij enkel haar eigen afspraken