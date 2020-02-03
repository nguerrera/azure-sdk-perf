FROM mcr.microsoft.com/dotnet/core/sdk AS build

# Copy csproj and restore
WORKDIR /app
COPY Directory.Build.props .
COPY Azure.Test.PerfStress/*.csproj Azure.Test.PerfStress/
COPY System.PerfStress/*.csproj System.PerfStress/

WORKDIR /app/System.PerfStress
RUN dotnet restore

# Copy code and publish
WORKDIR /app
COPY Azure.Test.PerfStress Azure.Test.PerfStress/
COPY System.PerfStress System.PerfStress/

WORKDIR /app/System.PerfStress
RUN dotnet publish --no-restore -c Release -o out -f netcoreapp2.0


# Copy build artifacts into runtime image
FROM mcr.microsoft.com/dotnet/core/runtime AS runtime
WORKDIR /app
COPY --from=build /app/System.PerfStress/out ./
ENTRYPOINT ["dotnet", "./System.PerfStress.dll"]
