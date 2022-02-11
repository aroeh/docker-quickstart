# Introduction

Welcome to the docker getting started repo.  We're going to be working with docker and docker compose to build and run a basic application.

## Goals

1. Learn docker basics
2. Explore some advanced topics like multi-container apps
3. Work with Docker Compose
4. Get an application running in Docker

## References

- [Docker Getting Started](https://docs.docker.com/get-started/)
- [Dockerize an ASP.Net Core Application](https://docs.docker.com/samples/dotnetcore/)
- [Github Docker DotNet](https://github.com/dotnet/dotnet-docker)
- [Docker Hub](https://hub.docker.com/)
    - [Docker Microsoft Images](https://hub.docker.com/publishers/microsoftowner)
    - [Docker .NET Images](https://hub.docker.com/_/microsoft-dotnet)
- [Docker Basic ASP.Net Samples](https://github.com/dotnet/dotnet-docker/tree/main/samples/aspnetapp)
- [Tutorial: Containerize a .NET App](https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows)
- [Hosting ASP.NET Core Images with Docker over HTTPS](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-6.0)
- [Getting a .NET Core WebAPI project ready for Docker](https://microsoft.github.io/AzureTipsAndTricks/blog/tip54.html)
- [Containerizing Blazor Apps](https://chrissainty.com/containerising-blazor-applications-with-docker-containerising-a-blazor-server-app/)
- [Redis cache in a dotnet core docker container](https://dotnetcorecentral.com/blog/redis-cache-in-net-core-docker-container/)
- [Redis cache with Docker Compose](https://geshan.com.np/blog/2022/01/redis-docker/)
- [Redis DotNet References](https://docs.redis.com/latest/rs/references/client_references/client_csharp/)
- [nginx](https://nginx.org/)
- [Hosting ASP.NET Core with nginx](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-6.0)

# Tools

- Windows 10 Pro **Pro or higher is needed since that enables the hyper-v virtualization on windows hosts
- Visual Studio 2022 v17.0.5
- Visual Studio Code v1.63.2
- Docker
    - Desktop v4.4.4
    - Engine v20.10.12
    - Compose v1.29.2
- .Net6

# Solution Overview

There are 4 projects in the solution
- quickstart_blazor_app - basic UI application which pulls data from the apis
- quickstart_api - default .net api template that returns randomized weather data
- quickstart_cats_api - basic api using cats as the data
- quickstart_lib - share class library used by the other projects

Each api project can be run independently, however the quickstart_blazor_app has dependencies on the apis.

# Build and Run

Each project can be built independently but docker and docker compose are the intended ways build all of the projects.  Use a Command line tool that can run docker commands

1. Make sure Docker is running
2. Change to the solution directory containing the .sln file
```
cd <PATH>\docker-quickstart\docker_quickstart_sln
```
3. Run docker compose up
```
docker-compose up
```

> NOTE: To run in detached mode use the following command
```
docker-compose up -d
```

4. View the apps running in Docker

> NOTE: The redirect for the blazor app here isn't perfect and doesn't work yet on http://localhost:4002
> The redirect does work for http://localhost:4001

If making any changes to a project, the container and image will need to rebuilt.  I found it easiest to remove all images using docker compose down
```
docker-compose down --rmi 'all'
```

## Docker Context

Context is a feature of the docker builds and docker compose that needs to be understood.  I wanted to call it out here because much documentation assumes that new users are aware of it, which might be the case.  The context is also the key in understanding the composition of the Dockerfile and docker-compose.yml

So what is context within docker and docker compose?  Context refers to the scope of the build directories in the docker deamon. In other words, context is the base directory docker will use when building and producing images.  Each Dockerfile and docker-compose.yml needs to have a plan for context since that determines configuration for key functions like COPY.

> The Context for this repository is set at the solution directory: docker_quickstart_sln
> All project Dockerfile are configured with this in mind

Let's take a look at the docker-compose.yml for quickstart_api.  We'll also look at that Dockerfile as well.

This is the entry within the docker-compose.yml for quickstart_api.
```
quickstart_api:
    image: ${DOCKER_REGISTRY-}weatherapi
    build:
      context: .
      dockerfile: quickstart_api/Dockerfile
```

Note the context and dockerfile.  context is set to "." meaning that the directory containing the docker-compose.yml is the context.  In other words the directory docker_quickstart_sln is the build context.  This also acts as the base for the location of the dockerfile.  Since the solution folder is the context, we point to the directory and specific docker file to use.  So the location fo the docker file entry ultimately runs as docker_quickstart_sln/quickstart_api/Dockerfile

So let's break down the dockerfile bit more to further understand context.  We'll use this first block as the example
```
# setting up the build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy each project file into working directory to restore dependencies
# starting out only copy the quickstart_api and quickstart_lib projects
COPY quickstart_api/*.csproj ./quickstart_api/
```

| Step | Line | What does it do? |
|------|------|------------------|
| 1 | FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build | Pulls the dotnet image and sets that with an alias named "build" |
| 2 | WORKDIR /source | Within the build image this creates a directory named "source" and sets that as the current directory for the image commands |
| 3 | COPY quickstart_api/*.csproj ./quickstart_api/ | Copies all csproj files from the build context location to the image directory.  In this instance it is copying docker_quickstart_sln/quickstart_api/quickstart_api.csproj to build/source/quickstart_api |

# Docker Commands

Review the docker command line reference at [Docker Docs](https://docs.docker.com/reference/)

These are the most common commands I found myself using in this project
| Command | Example | Reference Documentation | Notes |
|---------|---------|-------------------------|-------|
| docker-compose up [options] | docker-compose up -d | [Docker Compose Up](https://docs.docker.com/compose/reference/up/) | adding the -d will run in detached mode and is great for faster startup.  This is also preferred if not needing to debug |
| docker-compose down [options] | docker-compose down --rmi 'all'| [Docker Compose Down](https://docs.docker.com/compose/reference/down/) | When needing to make changes to a project you will have to rebuild the image and container.  This command made it easy to tear everything down very quickly to a clean state.  There might be a better way to do this though, but this was easy to use |
| docker logs -f <container-name> | docker logs -f docker_quickstart_sln-cache-1 | [Docker Logs](https://docs.docker.com/engine/reference/commandline/logs/) | If the container is nested under a solution, then use the nester container name and not the name of the image as set in the compose file |
| docker run [options] <image-name> |docker run redis| [Docker Logs](https://docs.docker.com/engine/reference/commandline/logs/) | This was handy for pulling down a single image to do some basic setup and testing before incorporating into the project and compose |