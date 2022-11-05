import {ChangeBlogManagementApi} from "../../clients/ChangeBlogManagementApiClient";
import ErrorMessages = ChangeBlogManagementApi.ErrorMessages;

export type Loading = {
    state: 'loading'
};

export type Success<T> = {
    state: 'success';
    account: T;
};

export type NotFound = {
    state: 'not-found';
};

export type UnknownError = {
    state: 'unknown-error';
    errorDetails: ErrorMessages[];
};

export type Resource<T> =
    | Loading
    | Success<T>
    | NotFound
    | UnknownError;