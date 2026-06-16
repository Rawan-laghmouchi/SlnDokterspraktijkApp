Feature: Afspraak maken bij een specifieke dokter

Als patiënt 
wil ik een afspraak kunnen maken bij een specifieke dokter
zodat ik op het gewenste moment een consultatie kan krijgen

@tag1
Scenario: Een patiënt maakt succesvol een afspraak op een beschikbaar tijdslot 
	Given dokter Timmermans heeft op 15-07-2026 een beschikbaar tijdslot om 10:30
	When patiënt Sara Peeters een afspraak maakt bij dokter Timmermans op 15-07-2026 om 10:30 voor een consultatie
	Then wordt de afspraak geregistreerd
	And is het tijdslot van 10:30 niet langer beschikbaar

Scenario: Een patiënt kan geen afspraak maken op een niet-beschikbaar tijdslot
	Given dokter Timmermans heeft op 15-07-2026 een niet-beschikbaar tijdslot om 09:30
	When patiënt Sara Peeters een afspraak probeert te maken bij dokter Timmermans op 15-07-2026 om 09:30 voor een consultatie
	Then wordt de afspraak geweigerd
	And krijgt zij de melding dat het gekozen tijdslot niet beschikbaar is