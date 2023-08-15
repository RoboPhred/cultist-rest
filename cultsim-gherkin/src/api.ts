import { CultistAPI, APIError } from "cultsim-api";

export const api = new CultistAPI("http://localhost:8081");

export function throwForStatus(err: Error, messages: Record<number, string>) {
  if (err instanceof APIError) {
    const message = messages[err.statusCode];
    if (message) {
      throw new Error(
        `${message} (server returned ${err.statusCode} ${err.message})`
      );
    }
  } else {
    console.log(err, "is not api error");
  }
}
