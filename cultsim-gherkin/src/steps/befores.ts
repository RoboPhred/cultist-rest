import path from "path";
import { readFileSync } from "fs";
import { fileURLToPath } from "url";

import { Before } from "@cucumber/cucumber";

import { post, put } from "../api.js";

const dirName = path.dirname(fileURLToPath(import.meta.url));
const emptyGameState = JSON.parse(
  readFileSync(path.join(dirName, "../../game-states/empty.json"), "utf8")
);

Before("not (@preservePreviousState or @noStateReset)", async () => {
  await put("/game-state", {
    gameState: emptyGameState,
  });
});

Before(async () => {
  await post("/time/speed", { speed: "Paused" });
});
