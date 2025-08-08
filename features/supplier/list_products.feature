Feature: List Products
  As an authenticated user
  I want to view a list of registered products
  So that I can see all available products in the system

  Scenario: Successfully list products
    Given I am authenticated with a valid JWT token
    When I request the list of products from the product listing endpoint
    Then I receive a list of registered products

  Scenario: List products when none exist
    Given I am authenticated with a valid JWT token
    And there are no products registered in the system
    When I request the list of products from the product listing endpoint
    Then I receive an empty list

  Scenario: List products without authentication
    Given I am not authenticated
    When I request the list of products from the product listing endpoint
    Then I do not receive the list of products
    And I receive an error message indicating authentication is requiredß