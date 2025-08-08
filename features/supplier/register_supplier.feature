Feature: Supplier Registration
  As an administrator
  I want to register new suppliers in the system
  So that I can manage supplier information and associate deliveries

  Scenario: Successful supplier registration
    Given I am authenticated with a valid JWT token
    And I am on the supplier registration endpoint
    When I submit valid supplier details
    Then the supplier is created
    And I receive a confirmation message

  Scenario: Registration with missing required fields
    Given I am authenticated with a valid JWT token
    And I am on the supplier registration endpoint
    When I submit incomplete supplier details
    Then the supplier is not created
    And I receive an error message describing the missing fields

  Scenario: Registration with duplicate supplier name
    Given a supplier with name "Acme Inc" is already registered
    And I am authenticated with a valid JWT token
    When I attempt to register another supplier with the name "Acme Inc"
    Then the supplier is not created
    And I receive an error message indicating the supplier already exists

  Scenario: Registration without authentication
    Given I am not authenticated
    When I attempt to register a new supplier
    Then the supplier is not created
    And I receive an error message indicating authentication is required