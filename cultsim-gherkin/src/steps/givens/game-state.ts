import { Given } from "@cucumber/cucumber";

import { post } from "../../api.js";

Given(/^I start a new (\S+) legacy$/, async (legacyId: string) => {
  await post("game-state/new-legacy", {
    legacyId,
  });
});
