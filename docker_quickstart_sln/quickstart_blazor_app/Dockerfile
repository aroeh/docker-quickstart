# setting up the build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy each project file into working directory to restore dependencies
# starting out only copy the quickstart_api and quickstart_lib projects
COPY quickstart_blazor_app/*.csproj ./quickstart_blazor_app/
COPY quickstart_lib/*.csproj ./quickstart_lib/

# restore packages on the project api project
RUN dotnet restore quickstart_blazor_app/quickstart_blazor_app.csproj

# copy the remaining api and lib project files
# can also do a shorthand command ex: COPY . .
COPY quickstart_blazor_app/. ./quickstart_blazor_app/
COPY quickstart_lib/. ./quickstart_lib/

# set the working directory to /source/project
WORKDIR  /source/quickstart_blazor_app

# build the api and lib projects for release configuration
RUN dotnet build -c release --no-restore

# Get the files from the build and publish them for release
# output the published files into the /app directory
FROM build AS publish
RUN dotnet publish -c release -o /app

# setting up the staging image for running the app and set the current working directory as app
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# copy the published files from the app directory
COPY --from=publish /app/wwwroot .
COPY quickstart_blazor_app/nginx.conf /etc/nginx/nginx.conf