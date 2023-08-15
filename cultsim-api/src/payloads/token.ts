export interface GetTokenResponseBase {
  /** The ID of the token. */
  id: string;

  /** The path of the token. */
  path: string;

  /** The path of the sphere containing the token. */
  spherePath: string;

  /** The type of the payload associated with the token. */
  payloadType: string;
}

export interface PutTokenRequestBase {
  /** The path to set for the sphere containing the token. */
  spherePath: string;
}
