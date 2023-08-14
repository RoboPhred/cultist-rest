Feature: Aspirant Legacy
  Scenario: Start the Aspirant Legacy
    Given I start a new aspirant legacy
    Then the tabletop should have the following cards:
      | elementId | quantity |
      | introjob  | 1        |
    And the work verb should be available

  @preservePreviousState
  Scenario: Begin the Menial Employment
    When I drag the introjob card to the work verb work slot
    And I start the work verb
    Then the started recipe should be workintrojob
    And the work verb should have 10 seconds remaining

  @preservePreviousState
  Scenario: Complete the Menial Employment
    When 10 seconds have elapsed
    Then the dream verb should be available
    And the dream verb should be on the introdream recipe
    And the dream verb should have 20 seconds remaining
    And the work verb should contain the following output:
      | elementId | quantity |
      | health    | 1        |
      | funds     | 2        |

  @preservePreviousState
  Scenario: Finish dreaming
    Given all situations are concluded
    When 20 seconds have elapsed
    Then the dream verb should contain the following output:
      | elementId   | quantity |
      | contentment | 1        |
      | passion     | 1        |
    And the study verb should be available
    And the study verb should be on the bequestcountdown recipe
    And the study verb should have 40 seconds remaining

  @preservePreviousState
  # FIXME: This will fail if this is not the first time the test has ran on the same save.
  # We need to start a new legacy at the start of this sequence
  # Or at least lobotomize seen recipes (and cards, and decks...).
  Scenario: The Bequest Arrival starts
    When 40 seconds have elapsed
    Then the study verb should be on the bequestarrives recipe
    And the study verb should have 30 seconds remaining

  @preservePreviousState
  Scenario: The Bequest Arrives
    When 30 seconds have elapsed
    Then the study verb should contain the following output:
      | elementId    | quantity |
      | reason       | 1        |
      | bequestintro | 1        |
      | funds        | 9        |