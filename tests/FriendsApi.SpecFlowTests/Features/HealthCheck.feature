Feature: HealthCheck
	I want to test my health check operation

@Read
Scenario: Health Check Call
	Given an http client
	When I Send a request to the health check operation
	Then I receive a success http code
