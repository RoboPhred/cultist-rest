import fetch from "cross-fetch";

import { APIError } from "./errors.js";
import {
  GetElementStackResponse,
  GetSituationResponse,
  GetTokenResponse,
  PutElementStackRequest,
  PutSituationRequest,
  GetSphereResponse,
  PostTokenRequest,
  GetLegacyResponse,
} from "./payloads/index.js";

// TODO: Wrap errors in additional errors explaining why things failed
// Invalid State Error - starting a verb that cant be started
// PathTargetError - Trying to execute a verb when targeting a token

export class CultistAPI {
  private baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public getLegacy(): Promise<GetLegacyResponse> {
    return request("GET", `${this.baseUrl}/api/game-state/legacy`);
  }

  public loadGameState(gameState: any): Promise<void> {
    return request("PUT", `${this.baseUrl}/api/game-state`, { gameState });
  }

  public loadLegacy(legacyId: string): Promise<void> {
    return request("PUT", `${this.baseUrl}/api/game-state/legacy`, {
      legacyId,
    });
  }

  public setSpeed(speed: "Paused" | "Normal" | "Fast" | "VeryFast") {
    return request("POST", `${this.baseUrl}/api/time/speed`, { speed });
  }

  public passTime(seconds: number) {
    return request("POST", `${this.baseUrl}/api/time/beat`, { seconds });
  }

  public getSpheresAtPath(fucinePath: string): Promise<GetSphereResponse[]> {
    return request("GET", `${this.baseUrl}/api/by-path/${fucinePath}/spheres`);
  }

  public async getTokensAtPath(
    fucinePath: string,
    query?: { payloadType?: "Situation" | "ElementStack"; entityId?: string }
  ): Promise<GetTokenResponse[]> {
    const qs = new URLSearchParams(query).toString();
    return request(
      "GET",
      `${this.baseUrl}/api/by-path/${fucinePath}/tokens?${qs}`
    );
  }

  public createTokensAtPath(
    fucinePath: string,
    tokenRequest: PostTokenRequest
  ): Promise<GetTokenResponse>;
  public createTokensAtPath(
    fucinePath: string,
    tokenRequests: PostTokenRequest[]
  ): Promise<GetTokenResponse[]>;
  public createTokensAtPath(
    fucinePath: string,
    tokenRequestOrRequests: PostTokenRequest | PostTokenRequest[]
  ): Promise<GetTokenResponse | GetTokenResponse[]> {
    return request(
      "POST",
      `${this.baseUrl}/api/by-path/${fucinePath}/tokens`,
      tokenRequestOrRequests
    );
  }

  public deleteAllTokensAtPath(fucinePath: string): Promise<void> {
    return request(
      "DELETE",
      `${this.baseUrl}/api/by-path/${fucinePath}/tokens`
    );
  }

  public updateTokenAtPath(
    fucinePath: string,
    updates: Partial<PutElementStackRequest>
  ): Promise<GetElementStackResponse>;
  public updateTokenAtPath(
    fucinePath: string,
    updates: Partial<PutSituationRequest>
  ): Promise<GetSituationResponse>;
  public updateTokenAtPath(
    fucinePath: string,
    updates: Partial<any>
  ): Promise<any> {
    return request(
      "PATCH",
      `${this.baseUrl}/api/by-path/${fucinePath}`,
      updates
    );
  }

  public deleteTokenAtPath(fucinePath: string): Promise<void> {
    return request("DELETE", `${this.baseUrl}/api/by-path/${fucinePath}`);
  }

  public executeSituationAtPath(situationPath: string) {
    return request(
      "POST",
      `${this.baseUrl}/api/by-path/${situationPath}/execute`
    );
  }

  public concludeSituationAtPath(situationPath: string) {
    return request(
      "POST",
      `${this.baseUrl}/api/by-path/${situationPath}/conclude`
    );
  }
}

async function request(method: string, url: string, body?: any): Promise<any> {
  const headers: Record<string, string> = {};
  if (body) {
    headers["Content-Type"] = "application/json";
  }
  const options: RequestInit = {
    method,
    body: body && JSON.stringify(body),
    headers,
  };

  const response = await fetch(url, options);

  if (response.status >= 400) {
    throw new APIError(url, response.status, await response.text());
  }

  if (response.headers.get("Content-Type")?.startsWith("application/json")) {
    return await response.json();
  }
}
