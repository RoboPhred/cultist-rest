import { Given } from "@cucumber/cucumber";
import HttpStatusCodes from "http-status-codes";

import { get, post, throwForStatus } from "../../api.js";

Given(
  /^I have a(?:n?) (\S+) verb on the (\S+) sphere$/,
  (verbId: string, spherePath: string) => {
    try {
      return post(`by-path/${spherePath}/tokens`, {
        payloadType: "Situation",
        verbId,
      });
    } catch (err: any) {
      throwForStatus(err, {
        [HttpStatusCodes.BAD_REQUEST]: `Cannot create a situation from verb id ${verbId} as the verb does not exist.`,
      });

      throw err;
    }
  }
);

Given(
  /^I have a(?:n?) (\S+) verb on the (\S+) sphere with the recipe (\S+)$/,
  (verbId: string, spherePath: string, recipeId: string) => {
    try {
      return post(`by-path/${spherePath}/tokens`, {
        payloadType: "Situation",
        verbId,
        recipeId,
      });
    } catch (err: any) {
      throwForStatus(err, {
        [HttpStatusCodes.BAD_REQUEST]: `Either the verb id ${verbId} does not exist, the recipe ${recipeId} does not exist, or the recipe ${recipeId} does not target the ${verbId} verb.`,
      });

      throw err;
    }
  }
);

Given("all situations are concluded", async () => {
  const situations = (await get(
    "/by-path/~/tabletop/tokens?payloadType=Situation"
  )) as any[];
  for (const situation of situations) {
    if (situation.state == "Complete") {
      await post(`/by-path/${situation.path}/conclude`, {});
    }
  }
});
