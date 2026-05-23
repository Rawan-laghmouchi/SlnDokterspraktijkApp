Feature: Een doktersattest downloaden

Als patiënt 
wil ik mijn dokterattest kunnen downloaden nadat de dokter dit heeft vrijgegeven
zodat ik het kan indienen bij school of werk

@tag1
Scenario: Een patiënt donwloadt een vrijgegeven dokterattest
	Given patiënt Rawan heeft een afgeronde consultatie 
	And dokter Timmermans heeft een doktersattest vrijgegeven
	When patiënt Rawan haar doktersattest downloadt
	Then ontvangt zij het doktersattest van deze consultatie

Scenario: Een patiênt kan geen niet-vrijgegeven attest downloaden
	Given patiënt Rawan heeft een afgeronde consultatie
	But dokter Timmermans heeft het doktersattest nog niet vrijgegeven
	When patiënt Rawan haar doktersattest probeert te downloaden
	Then wordt de download geweigerd

Scenario: Een patiënt kan een doktersattest niet opnieuw downloaden
Given patiënt Rawan heeft een afgeronde consultatie
And dokter Timmermans heeft een doktersattest vrijgegeven
And patiënt Rawan heeft haar doktersattest al gedownload
When patiënt Rawan haar doktersattest probeert te downloaden
Then wordt de download geweigerd