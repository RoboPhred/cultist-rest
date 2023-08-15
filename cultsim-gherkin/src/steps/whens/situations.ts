import { When } from "@cucumber/cucumber";

import { getVerbFromTabletopOrFail } from "../../utils.js";
import { CultistSimulatorWorld } from "../../world.js";
import { api } from "../../api.js";

When(
  /^I start the (\S+) verb$/,
  async function (this: CultistSimulatorWorld, verbId: string) {
    const verb = await getVerbFromTabletopOrFail(verbId);
    const { executedRecipeId } = await api.executeSituationAtPath(verb.path);
    this.startedRecipeId = executedRecipeId;
  }
);
