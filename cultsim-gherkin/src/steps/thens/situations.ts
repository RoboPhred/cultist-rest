import assert from "assert";

import { Then } from "@cucumber/cucumber";

import { CultistSimulatorWorld } from "../../world.js";

Then(
  /^the started recipe should be (\S+)$/,
  async function (this: CultistSimulatorWorld, expectedRecipeId: string) {
    assert.strictEqual(this.startedRecipeId, expectedRecipeId);
  }
);
