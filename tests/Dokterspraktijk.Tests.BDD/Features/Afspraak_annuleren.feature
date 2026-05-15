Feature: Een afspraak annuleren

Als patiënt 
wil ik een toekomstige afspraak kunnen annuleren
zodat ik een afspraak die ik niet meer nodig heb kan verwijderen

@tag1
Scenario: Een patiënt annuleert een toekomstige afspraak
	Given patiënt Rawan heeft een afspraak bij dokter Timmermans op 15-05-2026 om 10:30
	When zij deze afspraak annuleert
	Then wordt de afspraak geannuleerd
	And wordt het tijdslot opnieuw beschikbaar

Scenario: Een patiënt kan geen afgeronde afspraak annuleren
	Given patiënt Rawan heeft een afgeronde afspraak bij de dokter Timmermans op 10-05-2026 
	When zij deze afspraak probeert te annuleren 
	Then wordt de annulatie geweigerd