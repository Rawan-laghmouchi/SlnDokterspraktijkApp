Feature: Een doktersattest vrijgeven

Als dokter
wil ik een doktersattest kunnen vrijgeven na een consultatie
zodat de patiënt het attest kan downloaden

@tag1
Scenario: Een dokter geeft een attest vrij na een afgeronde consultatie
	Given dokter Timmermans heeft een afgeronde consultatie met patiënt Rawan
	And er is een doktersattest opgesteld voor deze consultatie
	When dokter Timmermans het doktersattest vrijgeeft
	Then wordt het attest beschikbaar voor de patiënt

Scenario: Een dokter kan geen attest vrijgeven voor een niet-afgeronde consultatie
	Given dokter Timmermans heeft een geplande consultatie met patiënt Rawan
	When dokter Timmermans een doktersattest probeert vrij te geven
	Then wordt deze actie geweigerd
