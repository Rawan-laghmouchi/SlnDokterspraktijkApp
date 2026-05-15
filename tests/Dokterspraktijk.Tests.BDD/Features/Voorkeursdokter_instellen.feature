Feature: Een voorkeursdokter instellen

Als patiënt 
wil ik een voorkeursdokter kunnen instellen
zodat ik bij nieuwe afspraken indien mogelijk bij dezelfde dokter terechtkom

@tag1
Scenario: Een patiënt stelt een voorkeursdokter in
	Given patiënt Rawan heeft nog geen voorkeursdokter
	When zij dokter Timmermans als voorkeursdokter instelt
	Then wordt dokter Timmermans bewaard als haar voorkeursdokter

Scenario: Een voorkeursdokter wordt gebruikt bij een nieuwe afspraak
	Given patiënt Rawan heeft dokter Timmermans als voorkeursdokter ingesteld
	And dokter Timmermans heeft op 20-05-2026 een beschikbaar tijdslot om 11:00
	When patiënt Rawan een nieuwe afspraak wil plannen op 20-05-2026
	Then wordt dokter Timmermans voorgesteld als voorkeursdokter

