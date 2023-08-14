import assert from "assert";

import { DataTable, Then } from "@cucumber/cucumber";

import { CultistSimulatorWorld } from "../../world.js";
import {
  elementAspectsMatch,
  getAllElementsFromSphere,
  getElementStackByElementIdFromSphereOrFail,
  getVerbFromTabletopOrFail,
} from "../../utils.js";

Then(
  /^the started recipe should be (\S+)$/,
  async function (this: CultistSimulatorWorld, expectedRecipeId: string) {
    assert.strictEqual(this.startedRecipeId, expectedRecipeId);
  }
);

Then(/^the (\S+) verb should be available$/, async (verbId: string) => {
  await getVerbFromTabletopOrFail(verbId);
});

Then(
  /^the (\S+) verb should have (\d+(?:\.\d+)?) second(?:s?) remaining$/,
  async (verbId: string, seconds: string) => {
    const situation = await getVerbFromTabletopOrFail(verbId);

    assert.equal(situation.timeRemaining, Number(seconds));
  }
);

Then(
  /^the (\S+) verb should be on the (\S+) recipe$/,
  async (verbId: string, recipeId: string) => {
    const situation = await getVerbFromTabletopOrFail(verbId);

    assert.equal(situation.recipeId, recipeId);
  }
);

Then(
  /^the (\S+) verb should contain the following output:$/,
  async (verbId: string, dataTable: DataTable) => {
    const situation = await getVerbFromTabletopOrFail(verbId);
    const targetSphere = situation.path + "/outputsphere";
    for (const item of dataTable.hashes()) {
      const { elementId, quantity } = item;
      const token = await getElementStackByElementIdFromSphereOrFail(
        elementId,
        targetSphere
      );
      assert.equal(token.quantity, Number(quantity));
    }
  }
);

Then(
  /^the (\S+) verb should contain an output card with the following aspects:$/,
  async (verbId: string, dataTable: DataTable) => {
    const situation = await getVerbFromTabletopOrFail(verbId);
    const targetSphere = situation.path + "/outputsphere";
    const elements = await getAllElementsFromSphere(targetSphere);

    const reqs = dataTable.hashes();
    const aspects = reqs.reduce((obj, req) => {
      obj[req.aspect] = Number(req.amount);
      return obj;
    }, {} as Record<string, number>);

    for (const element of elements) {
      if (elementAspectsMatch(element, aspects)) {
        return;
      }
    }

    throw new Error("No card in the sphere matched the aspects.");
  }
);
