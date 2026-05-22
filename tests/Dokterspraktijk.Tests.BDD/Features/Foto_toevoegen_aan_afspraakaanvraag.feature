Feature: Foto toevoegen aan afspraakaanvraag

Als patiënt
wil ik een foto kunnen toevoegen aan mijn afspraakaanvraag
zodat de dokter vooraf extra context heeft over mijn klacht

@tag1
Scenario: Een bestand wordt gevalideerd bij het toevoegen aan een afspraakaanvraag
	Given patiënt Rawan heeft een afspraakaanvraag voor een huidprobleem
	When zij een <bestandstype> toevoegt aan de afspraakaanvraag
	Then wordt het bestand <resultaat>

	Examples: 
	| bestandstype | resultaat    |
    | JPG-bestand  | geaccepteerd |
    | PNG-bestand  | geaccepteerd |
    | PDF-bestand  | geaccepteerd |
