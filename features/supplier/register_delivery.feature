Feature: Delivery Registration
  As an administrator
  I want to register a new delivery in the system
  So that I can track and manage deliveries for suppliers and products

  Scenario: Successful delivery registration
    Given I am authenticated with a valid JWT token
    And I am on the delivery registration endpoint
    And a supplier with name "Acme Inc" exists
    And a product with name "Widget Pro" exists
    When I submit valid delivery details associating "Acme Inc" and "Widget Pro"
    Then the delivery is created
    And I receive a confirmation message

  Scenario: Registration with missing required fields
    Given I am authenticated with a valid JWT token
    And I am on the delivery registration endpoint
    When I submit incomplete delivery details
    Then the delivery is not created
    And I receive an error message describing the missing fields

  Scenario: Registration with non-existent supplier or product
    Given I am authenticated with a valid JWT token
    And I am on the delivery registration endpoint
    When I submit delivery details with a supplier or product that does not exist
    Then the delivery is not created
    And I receive an error message indicating the supplier or product is invalid

  Scenario: Registration without authentication
    Given I am not authenticated
    When I attempt to register a new delivery
    Then the delivery is not created
    And I receive an error message indicating authentication is required