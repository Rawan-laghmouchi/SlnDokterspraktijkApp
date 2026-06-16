Feature: Komende afspraken bekijken

Als patiënt 
wil ik een overzicht kunnen zien van mijn komende afspraken met datum, dokter en reden van afspraak
zodat ik weet wanneer en waarvoor ik ingepland ben 

@tag1
Scenario: Een patiënt ziet haar komende afspraken
	Given patiënt Sara Peeters heeft de volgende afspraken:
	| Datum      | Tijd  | Dokter     | Reden                |
	| 15-07-2026 | 10:30 | Timmermans | algemene consultatie |
	| 20-07-2026 | 09:00 | Brancaert  | huidcontrole         |
	When zij haar komende afspraken raadpleegt
	Then ziet zij de volgende afspraken:
	| Datum      | Tijd  | Dokter     | Reden                |
	| 15-07-2026 | 10:30 | Timmermans | algemene consultatie |
	| 20-07-2026 | 09:00 | Brancaert  | huidcontrole         |

Scenario: Een patiënt ziet enkel haar eigen afspraken
	Given patiënt Sara Peeters heeft de volgende afspraken:
	| Datum      | Tijd  | Dokter     | Reden                |
	| 15-07-2026 | 10:30 | Timmermans | algemene consultatie |
	And patiënt Hans Vandenbogaerde heeft volgende afspraken:
	| Datum      | Tijd  | Dokter     | Reden                |
	| 20-07-2026 | 09:00 | Brancaert  | huidcontrole         |
	When patiënt Sara Peeters haar komende afspraken raadpleegt
	Then ziet zij enkel haar eigen afspraken