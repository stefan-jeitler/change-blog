FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
ARG Version
WORKDIR /app

COPY ./*.sln  ./

# Copy the main source project files
COPY src/*/*.*proj ./
RUN for file in $(ls *.*proj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# Copy the test project files
COPY tests/*/*.*proj ./
RUN for file in $(ls *.*proj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done

RUN dotnet restore

COPY . .
RUN dotnet publish src/ChangeTracker.Api/*.csproj \
  --configuration Release \
  --output publish \
  -p:Version=$Version \
  -NoLogo

RUN echo "Version ${Version}"

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "ChangeTracker.Api.dll", "--urls http://+:80"]
