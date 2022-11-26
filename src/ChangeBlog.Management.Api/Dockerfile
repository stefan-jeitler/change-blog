FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
ARG Version
WORKDIR /app

ENV NODE_VERSION=18.12.0
RUN apt install -y curl
RUN curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
ENV NVM_DIR=/root/.nvm
RUN . "$NVM_DIR/nvm.sh" && nvm install ${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm use v${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm alias default v${NODE_VERSION}
ENV PATH="/root/.nvm/versions/node/v${NODE_VERSION}/bin/:${PATH}"
RUN node --version
RUN npm --version

COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

RUN dotnet restore src/ChangeBlog.Management.Api/ChangeBlog.Management.Api.csproj

COPY . .
RUN dotnet publish ChangeBlog.Management.Api/ChangeBlog.Management.Api.csproj \
  --configuration Release \
  --output publish \
  -p:Version=$Version

RUN echo "Version ${Version}"

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ChangeBlog.Management.Api.dll", "--urls http://+:80"]
