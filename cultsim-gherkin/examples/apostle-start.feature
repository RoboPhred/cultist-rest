Feature: We can start the Apostle legacy
  Scenario: Begin the Menial Employment
    Given I have a introjob card on the ~tabletop sphere
    And I have a work verb on the ~tabletop sphere
    And I drag the introjob card to the work verb work slot
    When I start the work verb
    Then the started recipe should be workintrojob
