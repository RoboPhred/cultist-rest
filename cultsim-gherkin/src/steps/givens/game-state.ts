import { Given } from "@cucumber/cucumber";

import { api } from "../../api.js";

Given(/^I start a new (\S+) legacy$/, async (legacyId: string) => {
  await api.loadLegacy(legacyId);
});
