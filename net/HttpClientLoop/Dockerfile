FROM mcr.microsoft.com/dotnet/core/sdk:3.1.301 AS build

# Copy csproj and restore
WORKDIR /app
COPY Directory.Build.props .
COPY HttpClientLoop/*.csproj HttpClientLoop/

WORKDIR /app/HttpClientLoop
RUN dotnet restore

# Copy code and publish
WORKDIR /app
COPY HttpClientLoop HttpClientLoop/

WORKDIR /app/HttpClientLoop
RUN dotnet publish --no-restore -c Release -o out -f netcoreapp3.1


# Copy build artifacts into runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1.5 AS runtime
WORKDIR /app
COPY --from=build /app/HttpClientLoop/out ./
ENTRYPOINT ["dotnet", "./HttpClientLoop.dll"]
