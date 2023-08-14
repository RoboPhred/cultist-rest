import { Before } from "@cucumber/cucumber";

import { del, post } from "../api.js";

Before(async () => {
  await post("/time/speed", { speed: "Paused" });
  await del("/by-path/~/tabletop/tokens");
});
