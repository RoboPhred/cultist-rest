import fetch, { Response } from "node-fetch";

const endpoint = "http://localhost:8081/api";

export class APIError extends Error {
  public statusCode: number;

  constructor(statusCode: number, message: string) {
    super(`${statusCode}: ${message}`);
    this.name = this.constructor.name;
    this.statusCode = statusCode;

    // This is to ensure the instanceof check works correctly, since TypeScript transpilation might break prototype chain.
    Object.setPrototypeOf(this, APIError.prototype);
  }
}

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

export function getUrl(path: string) {
  if (path.startsWith("/")) {
    path = path.substring(1);
  }

  return `${endpoint}/${path}`;
}

export async function get(path: string) {
  const response = await fetch(getUrl(path), {
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
    },
  });

  return handleResponse(response);
}

export async function post(path: string, body: any) {
  const response = await fetch(getUrl(path), {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
    },
    body: JSON.stringify(body),
  });

  return handleResponse(response);
}

export async function put(path: string, body: any) {
  const response = await fetch(getUrl(path), {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
    },
    body: JSON.stringify(body),
  });

  return handleResponse(response);
}

export async function del(path: string) {
  const response = await fetch(getUrl(path), {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
    },
  });

  return handleResponse(response);
}

async function handleResponse(response: Response) {
  if (response.status >= 400) {
    throw new APIError(response.status, response.statusText);
  }

  if (response.headers.get("Content-Type")?.startsWith("application/json")) {
    return await response.json();
  }

  return null;
}
