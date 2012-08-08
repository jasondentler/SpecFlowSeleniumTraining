Feature: DisplayWidgets
	In order to sell widgets
	As a widget salesperson
	I want to display widgets on my website

@web
Scenario: Empty list of widgets
	When I view the widget list
	Then the error message is "Sorry. No widgets available."

@web
Scenario: List one widget
	Given I have added one widget
	When I view the widget list
	Then the widget details are displayed

@web
Scenario: List widgets
	Given I have added two widgets
	When I view the widget list
	Then two widgets are listed

@web
Scenario: Display a widget
	Given I have added a widget
	When I display the widget
	Then the widget is displayed

@web
Scenario: Widget not found
	When I display a widget that doesn't exist
	Then the error message is "Sorry. I couldn't find that widget."
