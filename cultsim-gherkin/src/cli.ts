#!/usr/bin/env node
import { fileURLToPath } from "url";
import path from "path";

const dirName = path.dirname(fileURLToPath(import.meta.url));

import {
  IRunConfiguration,
  loadConfiguration,
  runCucumber,
} from "@cucumber/cucumber/api";

async function run() {
  const { runConfiguration: loadedRunConfig } = await loadConfiguration();
  const runConfiguration: IRunConfiguration = {
    ...loadedRunConfig,
    sources: {
      ...loadedRunConfig.sources,
      paths: [...loadedRunConfig.sources.paths, "./**/*.feature"],
    },
    support: {
      ...loadedRunConfig.support,
      importPaths: [
        ...loadedRunConfig.support.importPaths,
        path.join(dirName, "bootstrap.js"),
        path.join(dirName, "steps", "befores.js"),
        path.join(dirName, "steps", "givens", "element-stacks.js"),
        path.join(dirName, "steps", "givens", "situations.js"),
        path.join(dirName, "steps", "whens", "situations.js"),
        path.join(dirName, "steps", "thens", "situations.js"),
      ],
    },
  };

  const result = await runCucumber(runConfiguration);

  if (!result.success) {
    process.exit(1);
  }
}

run();
