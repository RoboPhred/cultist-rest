import { Given } from "@cucumber/cucumber";

import { post } from "../../api.js";

Given(
  /^I have a(?:n?) (\S+) verb on the (\S+) sphere$/,
  (verbId: string, spherePath: string) => {
    return post(`by-path/${spherePath}/tokens`, {
      payloadType: "Situation",
      verbId,
    });
  }
);

Given(
  /^I have a(?:n?) (\S+) verb on the (\S+) sphere with the recipe (\S+)$/,
  (verbId: string, spherePath: string, recipeId: string) => {
    return post(`by-path/${spherePath}/tokens`, {
      payloadType: "Situation",
      verbId,
      recipeId,
    });
  }
);
