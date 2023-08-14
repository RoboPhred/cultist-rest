import { get } from "./api.js";

export async function getVerbFromTabletop(verbId: string) {
  const candidateVerbs = (await get(
    `by-path/~/tabletop/tokens?payloadType=Situation&entityId=${verbId}`
  )) as any[];

  if (!candidateVerbs.length) {
    throw new Error(`No verb found with id ${verbId}`);
  }

  return candidateVerbs[0];
}

export async function getVerbThresholdSphere(verbId: string, slotId: string) {
  const verb = await getVerbFromTabletop(verbId);
  const verbSpheres = (await get(`by-path/${verb.path}/spheres`)) as any[];
  const targetSphere = verbSpheres.find((x) => x.path.endsWith("/" + slotId));
  if (!targetSphere) {
    throw new Error(`No slot found with id ${slotId} in verb ${verbId}`);
  }

  return targetSphere;
}

export async function getElementStackByElementIdFromTabletop(
  elementId: string,
  spherePath: string
) {
  const candidates = (await get(
    `by-path/${spherePath}/tokens?entityId=${elementId}`
  )) as any[];
  if (!candidates.length) {
    throw new Error(
      `No element stack found with id ${elementId} in sphere ${spherePath}`
    );
  }

  return candidates[0];
}
