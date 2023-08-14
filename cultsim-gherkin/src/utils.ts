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
  elementId: string
) {}
