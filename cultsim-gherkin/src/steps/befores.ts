import { Before } from "@cucumber/cucumber";

import { del, post } from "../api.js";

Before("not @preservePreviousState", async () => {
  await post("/time/speed", { speed: "Paused" });
  await del("/by-path/~/tabletop/tokens");
});
