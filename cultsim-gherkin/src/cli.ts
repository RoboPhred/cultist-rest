#!/usr/bin/env node
import { fileURLToPath } from "url";
import path from "path";
import minimist from "minimist";

const dirName = path.dirname(fileURLToPath(import.meta.url));

const argv = minimist(process.argv.slice(2));

const names: string[] =
  argv["name"] && Array.isArray(argv["name"]) ? argv["name"] : [argv["name"]];

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
      names: names ?? loadedRunConfig.sources.names,
      paths: [...loadedRunConfig.sources.paths, "./**/*.feature"],
    },
    support: {
      ...loadedRunConfig.support,
      importPaths: [
        ...loadedRunConfig.support.importPaths,
        path.join(dirName, "bootstrap.js"),
        path.join(dirName, "steps", "**", "*.js"),
      ],
    },
  };

  const result = await runCucumber(runConfiguration);

  if (!result.success) {
    process.exit(1);
  }
}

run();
