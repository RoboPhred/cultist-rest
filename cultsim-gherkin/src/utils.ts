import { GetElementStackResponse, GetSituationResponse } from "cultsim-api";
import { api } from "./api.js";

export async function getVerbFromTabletopOrFail(verbId: string) {
  const candidateVerbs = await api.getTokensAtPath("~/tabletop", {
    payloadType: "Situation",
    entityId: verbId,
  });

  if (!candidateVerbs.length) {
    throw new Error(`No verb found with id ${verbId}`);
  }

  return candidateVerbs[0] as GetSituationResponse;
}

export async function getVerbThresholdSphereOrFail(
  verbId: string,
  slotId: string
) {
  const verb = await getVerbFromTabletopOrFail(verbId);
  const verbSpheres = await api.getSpheresAtPath(verb.path);

  const targetSphere = verbSpheres.find((x) => x.path.endsWith("/" + slotId));
  if (!targetSphere) {
    throw new Error(`No slot found with id ${slotId} in verb ${verbId}`);
  }

  return targetSphere;
}

export async function getAllVerbsFromTabletop() {
  return (await api.getTokensAtPath("~/tabletop", {
    payloadType: "Situation",
  })) as GetSituationResponse[];
}

export async function getAllElementsFromSphere(spherePath: string) {
  return api.getTokensAtPath(spherePath, { payloadType: "ElementStack" });
}

export async function getElementStackByElementIdFromSphereOrFail(
  elementId: string,
  spherePath: string
) {
  const candidates = await api.getTokensAtPath(spherePath, {
    payloadType: "ElementStack",
    entityId: elementId,
  });

  if (!candidates.length) {
    throw new Error(
      `No element stack found with id ${elementId} in sphere ${spherePath}`
    );
  }

  return candidates[0] as GetElementStackResponse;
}

export function elementAspectsMatch(
  element: any,
  aspects: Record<string, number>
) {
  for (const aspectName of Object.keys(aspects)) {
    const aspectAmount = aspects[aspectName];

    const elementAspectAmount =
      element.mutations[aspectName] ?? element.elementAspects[aspectName] ?? 0;

    if (elementAspectAmount !== aspectAmount) {
      return false;
    }
  }

  return true;
}
