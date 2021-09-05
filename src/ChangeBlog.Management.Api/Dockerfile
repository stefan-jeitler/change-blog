FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
ARG Version
WORKDIR /app

# Prevent 'Warning: apt-key output should not be parsed (stdout is not a terminal)'
ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=1

# install NodeJS 14.x
# see https://github.com/nodesource/distributions/blob/master/README.md#deb
RUN apt-get update -yq 
RUN apt-get install curl gnupg -yq 
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get install -y nodejs

COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

RUN dotnet restore src/ChangeBlog.Management.Api/ChangeBlog.Management.Api.csproj

COPY . .
RUN dotnet publish ChangeBlog.Management.Api/ChangeBlog.Management.Api.csproj \
  --configuration Release \
  --output publish \
  -p:Version=$Version \
  -NoLogo

RUN echo "Version ${Version}"

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ChangeBlog.Management.Api.dll", "--urls http://+:80"]
