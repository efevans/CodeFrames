FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build-env
WORKDIR /app

# get libman
COPY . ./
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli

# Restore libman packages
WORKDIR /app/CodeFramesAPI
RUN /root/.dotnet/tools/libman restore

# publish app
WORKDIR /app
RUN dotnet publish -c Release -o out

# start web app
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1
WORKDIR /app
COPY --from=build-env /app/CodeFramesAPI/out .
ENTRYPOINT ["dotnet", "CodeFramesAPI.dll"]