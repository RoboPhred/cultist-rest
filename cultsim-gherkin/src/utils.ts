import { get } from "./api.js";

export async function getVerbFromTabletopOrFail(verbId: string) {
  const candidateVerbs = (await get(
    `by-path/~/tabletop/tokens?payloadType=Situation&entityId=${verbId}`
  )) as any[];

  if (!candidateVerbs.length) {
    throw new Error(`No verb found with id ${verbId}`);
  }

  return candidateVerbs[0];
}

export async function getVerbThresholdSphereOrFail(
  verbId: string,
  slotId: string
) {
  const verb = await getVerbFromTabletopOrFail(verbId);
  const verbSpheres = (await get(`by-path/${verb.path}/spheres`)) as any[];
  const targetSphere = verbSpheres.find((x) => x.path.endsWith("/" + slotId));
  if (!targetSphere) {
    throw new Error(`No slot found with id ${slotId} in verb ${verbId}`);
  }

  return targetSphere;
}

export async function getAllElementsFromSphere(spherePath: string) {
  return (await get(
    `by-path/${spherePath}/tokens?payloadType=ElementStack`
  )) as any[];
}

export async function getElementStackByElementIdFromSphereOrFail(
  elementId: string,
  spherePath: string
) {
  const candidates = (await get(
    `by-path/${spherePath}/tokens?payloadType=ElementStack&entityId=${elementId}`
  )) as any[];
  if (!candidates.length) {
    throw new Error(
      `No element stack found with id ${elementId} in sphere ${spherePath}`
    );
  }

  return candidates[0];
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
