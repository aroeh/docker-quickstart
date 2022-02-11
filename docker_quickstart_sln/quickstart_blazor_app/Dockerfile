# setting up the build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy the .csproj from the context directory to the image directory
# copy docker_quickstart_sln/quickstart_blazor_app/*.csproj to build/source/quickstart_blazor_app
# copy quickstart_lib/quickstart_api/*.csproj to build/source/quickstart_lib
COPY quickstart_blazor_app/*.csproj ./quickstart_blazor_app/
COPY quickstart_lib/*.csproj ./quickstart_lib/

# In the build image - restore packages on the project api project
RUN dotnet restore quickstart_blazor_app/quickstart_blazor_app.csproj

# copy the remaining api and lib project files from the context directory to the image directory
COPY quickstart_blazor_app/. ./quickstart_blazor_app/
COPY quickstart_lib/. ./quickstart_lib/

# set the working directory to build/source/project
WORKDIR  /source/quickstart_blazor_app

# On the build image - build the api and lib projects for release configuration
RUN dotnet build -c release --no-restore

# Get the files from the build and publish them for release
# output the published files into the /app directory
FROM build AS publish
RUN dotnet publish -c release -o /app

# setting up the staging image for running the app and set the current working directory as app
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# copy the files from the published image app directory to the stage image app directory
COPY --from=publish /app/wwwroot .
COPY quickstart_blazor_app/nginx.conf /etc/nginx/nginx.conf