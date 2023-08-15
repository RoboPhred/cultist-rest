import { When } from "@cucumber/cucumber";

import { api } from "../../api.js";

When(/^(\d+(?:\.\d+)?) seconds have elapsed$/, async (seconds: string) => {
  await api.passTime(Number(seconds));
});
