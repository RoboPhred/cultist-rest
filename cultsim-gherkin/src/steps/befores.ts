import path from "path";
import { readFileSync } from "fs";
import { fileURLToPath } from "url";

import { Before } from "@cucumber/cucumber";

import { api } from "../api.js";

const dirName = path.dirname(fileURLToPath(import.meta.url));
const emptyGameState = JSON.parse(
  readFileSync(path.join(dirName, "../../game-states/empty.json"), "utf8")
);

Before("not (@preservePreviousState or @noStateReset)", async () => {
  await api.loadGameState(emptyGameState);
});

Before(async () => {
  await api.setSpeed("Paused");
});
