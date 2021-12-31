export const environment = {
    appVersion: require("../../package.json").version,
    production: true,
    authConfig: {
        issuer: "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0",
        tokenEndpoint: "https://login.microsoftonline.com/consumers/oauth2/v2.0/token",
        redirectUri: window.location.origin,
        logoutUrl: "https://login.microsoftonline.com/consumers/oauth2/v2.0/logout",
        oidc: true,
        clientId: "5fcc99b2-0300-42dd-9d45-5e0063b1257e",
        responseType: "code",
        scope: "openid email profile api://change-blog/.default",
        strictDiscoveryDocumentValidation: false,
        showDebugInformation: true
    }
};
