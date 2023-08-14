import { Given } from "@cucumber/cucumber";

import { get, post, put } from "../../api.js";
import { getVerbFromTabletop } from "../../utils.js";

Given(
  /^I have a(?:n?) (\S+) card on the (\S+) sphere$/,
  (elementId: string, spherePath: string) => {
    return post(`by-path/${spherePath}/tokens`, {
      payloadType: "ElementStack",
      elementId,
      quantity: 1,
    });
  }
);

Given(
  /^I have (\d+) (\S+) cards on the (\S+) sphere$/,
  (quantity: number, elementId: string, spherePath: string) => {
    post(`by-path/${spherePath}/tokens`, {
      elementId,
      quantity,
    });
  }
);

Given(
  /^I drag the (\S+) card from the (\S+) sphere to the (\S+) sphere$/,
  async (elementId: string, fromSpherePath: string, toSpherePath: string) => {
    const candidates = (await get(
      `by-path/${fromSpherePath}/tokens?entityId=${elementId}`
    )) as any[];
    if (!candidates.length) {
      throw new Error(
        `No element stack found with id ${elementId} in sphere ${fromSpherePath}`
      );
    }
    const targetToken = candidates[0].path;

    await put(`by-path/${targetToken}`, {
      spherePath: toSpherePath,
    });
  }
);

Given(
  /^I drag the (\S+) card to the (\S+) verb (\S+) slot$/,
  async (elementId: string, verbId: string, slot: string) => {
    const verbPath = (await getVerbFromTabletop(verbId)).path;

    const candidates = (await get(
      `by-path/~/tabletop/tokens?entityId=${elementId}`
    )) as any[];
    if (!candidates.length) {
      throw new Error(
        `No element stack found with id ${elementId} on the tabletop`
      );
    }
    const candidatePath = candidates[0].path;

    const verbSpheres = (await get(`by-path/${verbPath}/spheres`)) as any[];
    const targetSphere = verbSpheres.find((x) => x.path.endsWith("/" + slot));
    if (!targetSphere) {
      throw new Error(`No slot found with id ${slot} in verb ${verbId}`);
    }

    await put(`by-path/${candidatePath}`, {
      spherePath: targetSphere.path,
    });
  }
);
