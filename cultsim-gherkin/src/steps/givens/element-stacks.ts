import { Given } from "@cucumber/cucumber";
import HttpStatusCodes from "http-status-codes";

import { api, throwForStatus } from "../../api.js";
import {
  getElementStackByElementIdFromSphereOrFail,
  getVerbThresholdSphereOrFail,
} from "../../utils.js";
import { GetTokenResponse } from "cultsim-api";

Given(
  /^I have a(?:n?) (\S+) card on the tabletop$/,
  async (elementId: string) => {
    try {
      await api.createTokensAtPath("~/tabletop", {
        payloadType: "ElementStack",
        elementId,
        quantity: 1,
      });
    } catch (err: any) {
      throwForStatus(err, {
        [HttpStatusCodes.NOT_FOUND]: `Could not find the tabletop sphere.`,
        [HttpStatusCodes.BAD_REQUEST]: `The element ${elementId} could not be created.`,
      });

      throw err;
    }
  }
);

Given(
  /^I have (\d+) (\S+) card on the tabletop$/,
  async (quantity: string, elementId: string) => {
    try {
      await api.createTokensAtPath("~/tabletop", {
        payloadType: "ElementStack",
        elementId,
        quantity: Number(quantity),
      });
    } catch (err: any) {
      throwForStatus(err, {
        [HttpStatusCodes.NOT_FOUND]: `Could not find the tabletop sphere.`,
        [HttpStatusCodes.BAD_REQUEST]: `The element ${elementId} could not be created.`,
      });

      throw err;
    }
  }
);

Given(
  /^I have a(?:n?) (\S+) card on the (\S+) sphere$/,
  async (elementId: string, spherePath: string) => {
    try {
      await api.createTokensAtPath(spherePath, {
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
  async (quantity: number, elementId: string, spherePath: string) => {
    try {
      await api.createTokensAtPath(spherePath, {
        payloadType: "ElementStack",
        elementId,
        quantity: Number(quantity),
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
    let targetToken: GetTokenResponse;

    try {
      targetToken = await getElementStackByElementIdFromSphereOrFail(
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
      await api.updateTokenAtPath(targetToken.path, {
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
  async (elementId: string, verbId: string, slotId: string) => {
    const targetSphere = await getVerbThresholdSphereOrFail(verbId, slotId);

    const candidatePath = (
      await getElementStackByElementIdFromSphereOrFail(elementId, "~/tabletop")
    ).path as string;

    try {
      await api.updateTokenAtPath(candidatePath, {
        spherePath: targetSphere.path,
      });
    } catch (err: any) {
      throwForStatus(err, {
        [HttpStatusCodes.CONFLICT]: `The ${verbId} slot ${slotId} rejected the ${elementId} card.`,
      });

      throw err;
    }
  }
);
