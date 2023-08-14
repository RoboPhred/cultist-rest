import { When } from "@cucumber/cucumber";

import { post } from "../../api.js";

When(/^(\d+(?:\.\d+)?) seconds have elapsed$/, async (seconds: string) => {
  await post(`/time/beat`, {
    seconds,
  });
});
