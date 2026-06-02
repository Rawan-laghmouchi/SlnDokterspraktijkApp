Feature: Tijdsloten van een dokter raadplegen

Als patiënt 
wil ik de tijdsloten van een dokter op een bepaalde dag kunnen raadplegen
zodat ik een afspraak kan plannen op een beschikbaar moment

@tag1
Scenario: De patiënt raadpleegt de tijdsloten van een dokter op een bepaalde dag kunnen raadplegen
	Given dokter Timmermans heeft op 15-05-2026 de volgende tijdsloten:
	| Tijd  | Status          |
	| 09:00 | beschikbaar     |
	| 09:30 | niet beschikbaar|
	| 10:00 | beschikbaar     |
	When patiënt Rawan Laghmouchi de tijdsloten van dokter Timmermans op 15-05-2026 raadpleegt
	Then ziet zij de volgende tijdsloten:
	| Tijd  | Status          |
	| 09:00 | beschikbaar     |
	| 09:30 | niet beschikbaar|
	| 10:00 | beschikbaar     |
