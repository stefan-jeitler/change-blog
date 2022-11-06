import {ChangeBlogManagementApi} from "../../clients/ChangeBlogManagementApiClient";
import ErrorMessages = ChangeBlogManagementApi.ErrorMessages;

export type Success<T> = {
    state: 'success';
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
    | Success<T>
    | Loading
    | NotFound
    | Error;
