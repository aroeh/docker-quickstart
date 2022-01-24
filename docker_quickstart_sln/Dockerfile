# setting up the build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy solution from the root of the directory containing the dockerfile
COPY *.sln .

# copy the .csproj file from the app directory
COPY quickstart_api/*.csproj ./quickstart_api/

# restore packages on the project and solution
RUN dotnet restore

# copy the remaining project files
COPY quickstart_api/. ./quickstart_api/

# set the working directory to /source/project
WORKDIR  /source/quickstart_api

# build and publish the files for release into a directory named app
RUN dotnet publish -c release -o /app --no-restore

# setting up the staging image for running the app and set the current working directory as app
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# copy the published files from the build image app directory
COPY --from=build /app ./

# set the entry point command and dll for the app in the image
ENTRYPOINT ["dotnet", "quickstart_api.dll"]