export class APIError extends Error {
  public url: string;
  public statusCode: number;

  constructor(url: string, statusCode: number, message: string) {
    super(`${statusCode}: ${message} (at ${url})`);
    this.name = this.constructor.name;
    this.url = url;
    this.statusCode = statusCode;

    // This is to ensure the instanceof check works correctly, since TypeScript transpilation might break prototype chain.
    Object.setPrototypeOf(this, APIError.prototype);
  }
}
