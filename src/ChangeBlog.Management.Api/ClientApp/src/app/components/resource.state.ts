import {ChangeBlogManagementApi} from "../../clients/ChangeBlogManagementApiClient";
import ErrorMessages = ChangeBlogManagementApi.ErrorMessages;

export type Loaded<T> = {
  state: 'loaded';
  value: T;
};

export type Loading = {
  state: 'loading'
};

export type NotFound = {
  state: 'not-found';
};

export type Error = {
  state: 'error';
  errorDetails: ErrorMessages[];
};

export type Resource<T> =
  | Loaded<T>
  | Loading
  | NotFound
  | Error;
