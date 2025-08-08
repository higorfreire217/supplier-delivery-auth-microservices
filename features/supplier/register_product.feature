Feature: Product Registration
  As an administrator
  I want to register new products in the system
  So that I can manage product information and associate them with deliveries

  Scenario: Successful product registration
    Given I am authenticated with a valid JWT token
    And I am on the product registration endpoint
    When I submit valid product details
    Then the product is created
    And I receive a confirmation message

  Scenario: Registration with missing required fields
    Given I am authenticated with a valid JWT token
    And I am on the product registration endpoint
    When I submit incomplete product details
    Then the product is not created
    And I receive an error message describing the missing fields

  Scenario: Registration with duplicate product name
    Given a product with name "Widget Pro" is already registered
    And I am authenticated with a valid JWT token
    When I attempt to register another product with the name "Widget Pro"
    Then the product is not created
    And I receive an error message indicating the product already exists

  Scenario: Registration without authentication
    Given I am not authenticated
    When I attempt to register a new product
    Then the product is not created
    And I receive an error message indicating authentication is required