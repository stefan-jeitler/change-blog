FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
ARG Version
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish \
  --configuration Release \
  --output publish \
  -p:Version=$Version \
  -NoLogo

RUN echo "Version ${Version}"

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ChangeTracker.Api.dll", "--urls http://+:80"]
