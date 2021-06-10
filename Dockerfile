FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
ARG Version
WORKDIR /app

COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

RUN dotnet restore src/ChangeTracker.Api/ChangeTracker.Api.csproj

COPY . .
RUN dotnet publish src/ChangeTracker.Api/ChangeTracker.Api.csproj \
  --configuration Release \
  --output publish \
  -p:Version=$Version \
  -NoLogo

RUN echo "Version ${Version}"

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ChangeTracker.Api.dll", "--urls http://+:80"]
