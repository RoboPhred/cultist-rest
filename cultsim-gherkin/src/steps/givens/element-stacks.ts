import { Given } from "@cucumber/cucumber";
import HttpStatusCodes from "http-status-codes";

import { APIError, get, post, put, throwForStatus } from "../../api.js";
import {
  getElementStackByElementIdFromTabletop,
  getVerbFromTabletop,
} from "../../utils.js";

Given(
  /^I have a(?:n?) (\S+) card on the (\S+) sphere$/,
  (elementId: string, spherePath: string) => {
    try {
      return post(`by-path/${spherePath}/tokens`, {
        payloadType: "ElementStack",
        elementId,
        quantity: 1,
      });
    } catch (err: any) {
      throwForStatus(err, {
        [HttpStatusCodes.NOT_FOUND]: `Sphere ${spherePath} does not exist.`,
        [HttpStatusCodes.BAD_REQUEST]: `The element ${elementId} could not be created.`,
      });

      throw err;
    }
  }
);

Given(
  /^I have (\d+) (\S+) cards on the (\S+) sphere$/,
  (quantity: number, elementId: string, spherePath: string) => {
    if (quantity <= 0) {
      throw new Error("Quantity must be greater than zero.");
    }

    try {
      post(`by-path/${spherePath}/tokens`, {
        elementId,
        quantity,
      });
    } catch (err: any) {
      throwForStatus(err, {
        [HttpStatusCodes.NOT_FOUND]: `Sphere ${spherePath} does not exist.`,
        [HttpStatusCodes.BAD_REQUEST]: `The element ${elementId} could not be created.`,
      });

      throw err;
    }
  }
);

Given(
  /^I drag the (\S+) card from the (\S+) sphere to the (\S+) sphere$/,
  async (elementId: string, fromSpherePath: string, toSpherePath: string) => {
    let targetToken: any;

    try {
      targetToken = getElementStackByElementIdFromTabletop(
        elementId,
        fromSpherePath
      );
    } catch (err: any) {
      throwForStatus(err, {
        [HttpStatusCodes.NOT_FOUND]: `Source sphere ${fromSpherePath} does not exist.`,
      });

      throw err;
    }

    try {
      await put(`by-path/${targetToken}`, {
        spherePath: toSpherePath,
      });
    } catch (err: any) {
      throwForStatus(err, {
        [HttpStatusCodes.NOT_FOUND]: `Cannot move ${elementId} from ${fromSpherePath} to ${toSpherePath} because the source sphere does not exist.`,
        [HttpStatusCodes.CONFLICT]: `Cannot move ${elementId} from ${fromSpherePath} to ${toSpherePath} because the source sphere rejected it.`,
      });

      throw err;
    }
  }
);

Given(
  /^I drag the (\S+) card to the (\S+) verb (\S+) slot$/,
  async (elementId: string, verbId: string, slot: string) => {
    const verbPath = (await getVerbFromTabletop(verbId)).path;

    const candidatePath = getElementStackByElementIdFromTabletop(
      elementId,
      "~/tabletop"
    );

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
