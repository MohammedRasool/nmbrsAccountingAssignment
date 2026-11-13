Feature: Invoice overview
	As an accountant
	I want the invoice overview to be correct
	So that I can trust the totals

	Scenario: The summary row contains the correct total of all invoices
		Given I am on the "Invoices" page
		When I read all invoice amounts from the overview
		Then the summary row total equals the sum of those amounts

	Scenario: Invoice I634 shows the expected amount
		Given I am on the "Invoices" page
		When I read the amount of invoice "I634"
		Then the amount equals "423.99 EUR"