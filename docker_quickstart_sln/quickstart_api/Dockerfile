# setting up the build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy each project file into working directory to restore dependencies
# starting out only copy the quickstart_api and quickstart_lib projects
COPY quickstart_api/*.csproj ./quickstart_api/
COPY quickstart_lib/*.csproj ./quickstart_lib/

# restore packages on the project api project
RUN dotnet restore quickstart_api/quickstart_api.csproj

# copy the remaining api and lib project files
COPY quickstart_api/. ./quickstart_api/
COPY quickstart_lib/. ./quickstart_lib/

# set the working directory to /source/project
WORKDIR  /source/quickstart_api

# build the api and lib projects for release configuration
RUN dotnet build -c release --no-restore

# Get the files from the build and publish them for release
# output the published files into the /app directory
FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

# setting up the staging image for running the app and set the current working directory as app
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# copy the published files from the app directory
COPY --from=publish /app .

# set the entry point command and dll for the app in the image
ENTRYPOINT ["dotnet", "quickstart_api.dll"]