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
