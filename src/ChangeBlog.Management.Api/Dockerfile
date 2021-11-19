FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ARG Version
WORKDIR /app

RUN apt-get update -yq 
RUN apt-get install curl gnupg -yq 
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs

COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

RUN dotnet restore src/ChangeBlog.Management.Api/ChangeBlog.Management.Api.csproj

COPY . .
RUN dotnet publish ChangeBlog.Management.Api/ChangeBlog.Management.Api.csproj \
  --configuration Release \
  --output publish \
  -p:Version=$Version

RUN echo "Version ${Version}"

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ChangeBlog.Management.Api.dll", "--urls http://+:80"]
