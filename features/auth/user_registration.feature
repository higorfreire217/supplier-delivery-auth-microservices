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
    And And I receive an error message describing the missing registration fields

  Scenario: Registration with existing email
    Given I am on the registration endpoint
    And a user with email "john@example.com" is already registered
    When I attempt to register with the same email
    Then my account is not created
    And I receive an error message indicating the email is already in use

  Scenario: Registration with weak password
    Given I am on the registration endpoint
    When I submit a password that does not meet security requirements
    Then my account is not created
    And I receive an error message about password strength

  Scenario: Registration with invalid email format
    Given I am on the registration endpoint
    When I submit an email address that is not properly formatted
    Then my account is not created
    And I receive an error message about email format

  Scenario: Registration with name that is too short
    Given I am on the registration endpoint
    When I submit a name with less than 3 characters
    Then my account is not created
    And I receive an error message indicating the name is too short

  Scenario: Registration with name that is too long
    Given I am on the registration endpoint
    When I submit a name with more than 50 characters
    Then my account is not created
    And I receive an error message indicating the name is too long

  Scenario: Registration with email that is too short
    Given I am on the registration endpoint
    When I submit an email address with less than 8 characters
    Then my account is not created
    And I receive an error message indicating the email is too short

  Scenario: Registration with email that is too long
    Given I am on the registration endpoint
    When I submit an email address with more than 100 characters
    Then my account is not created
    And I receive an error message indicating the email is too long

  Scenario: Registration with password that is too short
    Given I am on the registration endpoint
    When I submit a password with less than 8 characters
    Then my account is not created
    And I receive an error message indicating the password is too short

  Scenario: Registration with password that is too long
    Given I am on the registration endpoint
    When I submit a password with more than 100 characters
    Then my account is not created
    And I receive an error message indicating the password is too long