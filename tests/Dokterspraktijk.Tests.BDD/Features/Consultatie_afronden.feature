Feature: Een Consultatie afronden

Als dokter
wil ik een consultatie kunnen afronden
zodat de afspraak de juiste status krijgt en vervolgacties mogelijk worden

@tag1
Scenario: Een dokter rondt een geplande consultatie af
	Given dokter Timmermans heeft een geplande afspraak met patiënt Rawan op 15-05-2026 om 10:30
	When dokter Timmermans de consultatie afrondt 
	Then krijgt de afspraak de status "Afgerond"

Scenario: Een reeds afgeronde consultatie kan niet opnieuw afgerond worden
	Given dokter Timmermans heeft een afspraak met status "Afgerond"
	When dokter Timmermans de consultatie opnieuw probeert af te ronden
	Then wordt deze actie geweigerd
