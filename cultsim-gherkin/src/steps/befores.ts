import { Before } from "@cucumber/cucumber";

import { APIError, del, get, post } from "../api.js";

Before(async () => {
  await post("/time/speed", { speed: "Paused" });
});

Before("not (@preservePreviousState or @noStateReset)", async () => {
  try {
    await del("/by-path/~/tabletop/tokens");
  } catch (err: any) {
    if (err instanceof APIError) {
      if (err.statusCode === 404 && err.message.includes("No sphere found")) {
        // Table doesnt exist.  That's fine.
        return;
      }
    }

    throw err;
  }
});
