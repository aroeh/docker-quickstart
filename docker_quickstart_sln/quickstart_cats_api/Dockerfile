# setting up the build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy the .csproj from the context directory to the image directory
# copy docker_quickstart_sln/quickstart_cats_api/*.csproj to build/source/quickstart_cats_api
# copy quickstart_lib/quickstart_api/*.csproj to build/source/quickstart_lib
COPY quickstart_cats_api/*.csproj ./quickstart_cats_api/
COPY quickstart_lib/*.csproj ./quickstart_lib/

# In the build image - restore packages on the project api project
RUN dotnet restore quickstart_cats_api/quickstart_cats_api.csproj

# copy the remaining api and lib project files from the context directory to the image directory
COPY quickstart_cats_api/. ./quickstart_cats_api/
COPY quickstart_lib/. ./quickstart_lib/

# set the working directory to build/source/project
WORKDIR  /source/quickstart_cats_api

# On the build image - build the api and lib projects for release configuration
RUN dotnet build -c release --no-restore

# Get the files from the build and publish them for release
# output the published files into the /app directory
FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

# setting up the staging image for running the app and set the current working directory as app
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# copy the files from the published image app directory to the stage image app directory
COPY --from=publish /app .

# set the entry point command and dll for the app in the image
ENTRYPOINT ["dotnet", "quickstart_cats_api.dll"]