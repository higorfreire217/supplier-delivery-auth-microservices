Feature: User Login
  As a registered user
  I want to log in to the platform
  So that I can access protected features

  Scenario: Successful user login
    Given I am a registered user with email "john@example.com" and a valid password
    When I submit my email and password to the login endpoint
    Then I receive a valid JWT token
    And I am authenticated

  Scenario: Login with incorrect password
    Given I am a registered user with email "john@example.com"
    When I submit my email and an incorrect password to the login endpoint
    Then I do not receive a JWT token
    And I receive an error message indicating invalid credentials

  Scenario: Login with unregistered email
    Given no user is registered with email "alice@example.com"
    When I attempt to log in with "alice@example.com" and any password
    Then I do not receive a JWT token
    And I receive an error message indicating invalid credentials

  Scenario: Login with missing required fields
    Given I am on the login endpoint
    When I submit a request missing the email or password
    Then I do not receive a JWT token
    And I receive an error message describing the missing login fields