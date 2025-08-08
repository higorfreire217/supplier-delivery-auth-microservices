Feature: Token Validation
  As an authenticated user
  I want my JWT token to be validated when accessing protected endpoints
  So that only authorized users can access secure resources

  Scenario: Access with valid token
    Given I have a valid JWT token
    When I access a protected endpoint
    Then my request is authorized
    And I receive the requested resource

  Scenario: Access with expired token
    Given I have an expired JWT token
    When I access a protected endpoint
    Then my request is denied
    And I receive an error message indicating the token has expired

  Scenario: Access with invalid token
    Given I have a malformed or tampered JWT token
    When I access a protected endpoint
    Then my request is denied
    And I receive an error message indicating the token is invalid

  Scenario: Access without token
    Given I do not provide any JWT token
    When I access a protected endpoint
    Then my request is denied
    And I receive an error message indicating authentication is required