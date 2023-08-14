import { When } from "@cucumber/cucumber";

import { getVerbFromTabletopOrFail } from "../../utils.js";
import { post } from "../../api.js";
import { CultistSimulatorWorld } from "../../world.js";

When(
  /^I start the (\S+) verb$/,
  async function (this: CultistSimulatorWorld, verbId: string) {
    const verbPath = (await getVerbFromTabletopOrFail(verbId)).path;
    const { executedRecipeId } = (await post(
      `by-path/${verbPath}/execute`,
      {}
    )) as any;
    this.startedRecipeId = executedRecipeId;
  }
);
