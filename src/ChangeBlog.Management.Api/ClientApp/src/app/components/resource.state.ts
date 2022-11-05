import {ChangeBlogManagementApi} from "../../clients/ChangeBlogManagementApiClient";
import IErrorMessages = ChangeBlogManagementApi.IErrorMessages;

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
    details: IErrorMessages[];
};

export type Resource<T> =
    | Loading
    | Success<T>
    | NotFound
    | UnknownError;