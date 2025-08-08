Feature: User Registration
  As a new user
  I want to register on the platform
  So that I can access protected features

  Scenario: Successful user registration
    Given I am on the registration endpoint
    When I submit valid user information
    Then my account is created
    And I receive a confirmation message

  Scenario: Registration with missing required fields
    Given I am on the registration endpoint
    When I submit incomplete user information
    Then my account is not created
    And I receive an error message describing the missing fields

  Scenario: Registration with existing email
    Given a user with email "john@example.com" is already registered
    When I attempt to register with the same email
    Then my account is not created
    And I receive an error message indicating the email is already in use

  Scenario: Registration with weak password
    Given I am on the registration endpoint
    When I submit a password that does not meet security requirements
    Then my account is not created
    And I receive an error message about password strength