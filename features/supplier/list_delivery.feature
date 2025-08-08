Feature: List Deliveries
  As an authenticated user
  I want to view a list of registered deliveries
  So that I can track and manage all deliveries in the system

  Scenario: Successfully list deliveries
    Given I am authenticated with a valid JWT token
    When I request the list of deliveries from the delivery listing endpoint
    Then I receive a list of registered deliveries

  Scenario: List deliveries when none exist
    Given I am authenticated with a valid JWT token
    And there are no deliveries registered in the system
    When I request the list of deliveries from the delivery listing endpoint
    Then I receive an empty list

  Scenario: List deliveries without authentication
    Given I am not authenticated
    When I request the list of deliveries from the delivery listing endpoint
    Then I do not receive the list of deliveries
    And I receive an error message indicating authentication is required