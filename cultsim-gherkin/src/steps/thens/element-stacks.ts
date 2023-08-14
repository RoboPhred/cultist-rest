import { DataTable, Then } from "@cucumber/cucumber";
import assert from "assert";

import { getElementStackByElementIdFromSphereOrFail } from "../../utils.js";

Then(
  /^the tabletop should have the following cards:$/,
  async (dataTable: DataTable) => {
    for (const item of dataTable.hashes()) {
      const { elementId, quantity } = item;
      const token = await getElementStackByElementIdFromSphereOrFail(
        elementId,
        "~/tabletop"
      );
      assert.equal(token.quantity, Number(quantity));
    }
  }
);
