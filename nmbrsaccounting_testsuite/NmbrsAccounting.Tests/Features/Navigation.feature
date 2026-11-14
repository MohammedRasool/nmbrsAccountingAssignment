Feature: Menu navigation
	As a user of Nmbrs Accounting
	I want to open every page from the menu
	So that I can be sure the application navigation works

	Scenario: Every page in the menu opens successfully
		Given I am on the "Home" page
		Then the page title should be "Welcome"
		When I visit every page listed in the menu
		Then every visited page loads successfully