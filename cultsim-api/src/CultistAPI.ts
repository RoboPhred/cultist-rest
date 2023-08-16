import fetch, { RequestInit } from "node-fetch";

import { APIError } from "./errors.js";
import {
  GetElementStackResponse,
  GetSituationResponse,
  GetTokenResponse,
  PutElementStackRequest,
  PutSituationRequest,
  GetSphereResponse,
  PostTokenRequest,
} from "./payloads/index.js";

// TODO: Wrap errors in additional errors explaining why things failed
// Invalid State Error - starting a verb that cant be started
// PathTargetError - Trying to execute a verb when targeting a token

export class CultistAPI {
  private baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public async loadGameState(gameState: any): Promise<void> {
    return request("PUT", `${this.baseUrl}/api/game-state`, { gameState });
  }

  public async loadLegacy(legacyId: string): Promise<void> {
    return request("PUT", `${this.baseUrl}/api/game-state/legacy`, {
      legacyId,
    });
  }

  public async setSpeed(speed: "Paused" | "Normal" | "Fast" | "VeryFast") {
    return request("POST", `${this.baseUrl}/api/time/speed`, { speed });
  }

  public async passTime(seconds: number) {
    return request("POST", `${this.baseUrl}/api/time/beat`, { seconds });
  }

  public async getSpheresAtPath(
    fucinePath: string
  ): Promise<GetSphereResponse[]> {
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

  public async createTokensAtPath(
    fucinePath: string,
    tokenRequest: PostTokenRequest
  ): Promise<GetTokenResponse>;
  public async createTokensAtPath(
    fucinePath: string,
    tokenRequests: PostTokenRequest[]
  ): Promise<GetTokenResponse[]>;
  public async createTokensAtPath(
    fucinePath: string,
    tokenRequestOrRequests: PostTokenRequest | PostTokenRequest[]
  ): Promise<GetTokenResponse | GetTokenResponse[]> {
    return request(
      "POST",
      `${this.baseUrl}/api/by-path/${fucinePath}/tokens`,
      tokenRequestOrRequests
    );
  }

  public async deleteAllTokensAtPath(fucinePath: string): Promise<void> {
    return request(
      "DELETE",
      `${this.baseUrl}/api/by-path/${fucinePath}/tokens`
    );
  }

  public async updateTokenAtPath(
    fucinePath: string,
    updates: Partial<PutElementStackRequest>
  ): Promise<GetElementStackResponse>;
  public async updateTokenAtPath(
    fucinePath: string,
    updates: Partial<PutSituationRequest>
  ): Promise<GetSituationResponse>;
  public async updateTokenAtPath(
    fucinePath: string,
    updates: Partial<any>
  ): Promise<any> {
    return request(
      "PATCH",
      `${this.baseUrl}/api/by-path/${fucinePath}`,
      updates
    );
  }

  public async deleteTokenAtPath(fucinePath: string): Promise<void> {
    return request("DELETE", `${this.baseUrl}/api/by-path/${fucinePath}`);
  }

  public async executeSituationAtPath(situationPath: string) {
    return request(
      "POST",
      `${this.baseUrl}/api/by-path/${situationPath}/execute`
    );
  }

  public async concludeSituationAtPath(situationPath: string) {
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
