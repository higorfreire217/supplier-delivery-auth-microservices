Feature: List Suppliers
  As an authenticated user
  I want to view a list of registered suppliers
  So that I can see all available suppliers in the system

  Scenario: Successfully list suppliers
    Given I am authenticated with a valid JWT token
    When I request the list of suppliers from the supplier listing endpoint
    Then I receive a list of registered suppliers

  Scenario: List suppliers when none exist
    Given I am authenticated with a valid JWT token
    And there are no suppliers registered in the system
    When I request the list of suppliers from the supplier listing endpoint
    Then I receive an empty list

  Scenario: List suppliers without authentication
    Given I am not authenticated
    When I request the list of suppliers from the supplier listing endpoint
    Then I do not receive the list of suppliers
    And I receive an error message indicating authentication is required