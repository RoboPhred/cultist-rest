import { GetTokenResponseBase, PutTokenRequestBase } from "./token";

export interface GetSituationResponse extends GetTokenResponseBase {
  payloadType: "Situation";

  /** The ID of the verb associated with the situation. */
  verbId: string;

  /** Whether the verb will disappear once completed. */
  spontaneous: boolean;

  /** The remaining time for the situation. */
  timeRemaining: number;

  /** The ID of the fallback recipe for the situation. */
  recipeId: string | null;

  /** The label of the fallback recipe for the situation. */
  recipeLabel: string | null;

  /** The ID of the current recipe for the situation. */
  currentRecipeId: string | null;

  /** The label of the current recipe for the situation. */
  currentRecipeLabel: string | null;

  /** The state of the situation. */
  state: string;

  /** The icon of the situation. */
  icon: string;

  /** The label of the situation. */
  label: string;

  /** The description of the situation. */
  description: string;

  /** Indicates if the situation is open. */
  open: boolean;
}

export interface PutSituationRequest extends PutTokenRequestBase {
  /** The ID of the recipe to set for the situation. */
  recipeId: string;

  /** Indicates if the situation should be open. */
  open: boolean;
}

export interface PostSituationRequestVerbId {
  payloadType: "Situation";

  /** The ID of the verb to tie to this situation. */
  verbId: string;
}

export interface PostSituationRequestRecipeId {
  payloadType: "Situation";

  /** The recipe ID to initialize the verb with. */
  recipeId: string;
}

export type PostSituationRequest =
  | PostSituationRequestVerbId
  | PostSituationRequestRecipeId
  | (PostSituationRequestVerbId & PostSituationRequestRecipeId);

export interface PostExecuteRecipeResponse {
  executedRecipeId: string;
  executedRecipeLabel: string;
}
