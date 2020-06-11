Feature: IBAN_testing
	IBAN validation will be tested 
	Using POST API requests 
	With negative and positive expected results

@test_validation_positive
Scenario: Send POST request with correct JWT
	Given I have sent request using IBAN and token provided
	| iban                   | token                                    |
	| GB09HAOE91311808002317 | Q7DaxRnFls6IpwSW1SQ2FaTFOf7UdReAFNoKY68L |	
	Then the response should be reported	
	And the correct test result should be returned
	| error_code |
	| Ok       |

@test_validation_negative
Scenario: Send POST request without correct JWT
	Given I have sent request using IBAN and token provided
	| iban                   | token                                    |
	| GB09HAOE91311808002317 |                                          |	 
	Then the response should be reported
	And the correct test result should be returned
	| error_code |
	| Unauthorized |
	