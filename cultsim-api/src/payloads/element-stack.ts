import { Aspects } from "../types";
import { GetTokenResponseBase, PutTokenRequestBase } from "./token";

export interface GetElementStackResponse extends GetTokenResponseBase {
  payloadType: "ElementStack";

  /** The ID of the element. */
  elementId: string;

  /** The quantity of the element stack. */
  quantity: number;

  /** The remaining lifetime of the element stack. */
  lifetimeRemaining: number;

  /** The aspects of the element. */
  elementAspects: Aspects;

  /** The mutations of the element stack. */
  mutations: Aspects;

  /** Indicates if the element stack is shrouded. */
  shrouded: boolean;

  /** The label of the element stack. */
  label: string;

  /** The description of the element stack. */
  description: string;

  /** The icon of the element stack. */
  icon: string;

  /** The uniqueness group of the element stack. */
  uniquenessGroup: string | null;

  /** Indicates if the element stack decays. */
  decays: boolean;

  /** Indicates if the element stack is metafictional. */
  metafictional: boolean;

  /** Indicates if the element stack is unique. */
  unique: boolean;
}

export interface PutElementStackRequest extends PutTokenRequestBase {
  /** The quantity to set for the element stack. */
  quantity: number;

  /** The mutations to set for the element stack. */
  mutations: Aspects;

  /** Indicates if the element stack should be shrouded. */
  shrouded: boolean;
}

export interface PostElementStackRequest {
  payloadType: "ElementStack";

  /** The ID of the element to create. */
  elementId: string;

  /** The quantity of the stack. */
  quantity: number;

  /** Mutations to apply to the new stack. */
  mutations?: Aspects;
}
