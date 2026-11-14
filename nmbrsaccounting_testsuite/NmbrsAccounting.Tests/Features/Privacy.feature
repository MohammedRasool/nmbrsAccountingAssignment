Feature: Privacy page
	As a user of Nmbrs Accounting
	I want to view the privacy policy
	So that I understand how my data is handled

	Scenario: Privacy page displays correct content
		Given I am on the "Privacy" page
		Then the page title should be "Privacy Policy"
