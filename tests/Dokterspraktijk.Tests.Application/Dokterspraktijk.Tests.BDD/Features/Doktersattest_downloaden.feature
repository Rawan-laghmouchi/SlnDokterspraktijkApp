Feature: Een doktersattest downloaden

Als patiënt 
wil ik mijn doktersattest kunnen downloaden nadat de dokter dit heeft vrijgegeven
zodat ik het kan indienen bij school of werk

@tag1
Scenario: Een patiënt downloadt een vrijgegeven doktersattest
	Given patiënt Sara Peeters heeft een afgeronde consultatie 
	And dokter Timmermans heeft een doktersattest vrijgegeven
	When patiënt Sara Peeters haar doktersattest downloadt
	Then ontvangt zij het doktersattest van deze consultatie

Scenario: Een patiënt kan geen niet-vrijgegeven attest downloaden
	Given patiënt Sara Peeters heeft een afgeronde consultatie
	But dokter Timmermans heeft het doktersattest nog niet vrijgegeven
	When patiënt Sara Peeters haar doktersattest probeert te downloaden
	Then wordt de download geweigerd