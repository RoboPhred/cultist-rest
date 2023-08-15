export * from "./element-stack.js";
export * from "./situation.js";
export * from "./sphere.js";
export * from "./token.js";

import {
  GetElementStackResponse,
  PostElementStackRequest,
  PutElementStackRequest,
} from "./element-stack.js";
import {
  GetSituationResponse,
  PostSituationRequest,
  PutSituationRequest,
} from "./situation.js";

export type GetTokenResponse = GetElementStackResponse | GetSituationResponse;
export type PutTokenRequest = PutElementStackRequest | PutSituationRequest;
export type PostTokenRequest = PostElementStackRequest | PostSituationRequest;
