﻿FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
ARG Version
WORKDIR /app

COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

RUN dotnet restore src/ChangeBlog.Api/ChangeBlog.Api.csproj

COPY . .
RUN dotnet publish ChangeBlog.Api/ChangeBlog.Api.csproj \
  --configuration Release \
  --output publish \
  -p:Version=$Version

RUN echo "Version ${Version}"

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ChangeBlog.Api.dll", "--urls http://+:80"]
