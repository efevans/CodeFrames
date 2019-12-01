FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build-env

# copy files
WORKDIR /app
COPY . ./

# publish app
WORKDIR /app
RUN dotnet publish -c Release -o out

# start web app
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1
WORKDIR /app
COPY --from=build-env /app/CodeFramesAPI/out .
ENTRYPOINT ["dotnet", "CodeFramesAPI.dll"]