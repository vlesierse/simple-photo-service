import { CognitoUser } from "amazon-cognito-identity-js";
import { AuthError as AmpAuthError } from "@aws-amplify/auth/src/Errors";

export type User = CognitoUser & {
  attributes: { [key: string]: string };
};

export type AuthError = unknown & AmpAuthError;
