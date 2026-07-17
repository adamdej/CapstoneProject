Feature: Corporate Wellness Form Validation
  As a Practo user
  I want the Corporate Wellness form to validate my input
  So that I cannot submit incorrect contact details

  Scenario: Invalid contact number disables the submit button
    Given I am on the Corporate Wellness page
    When I enter an invalid contact number
    Then the contact number field should be marked invalid
    And the submit button should remain disabled