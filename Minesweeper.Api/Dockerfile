# Build context is the repo root (the API pulls in Core + Infrastructure via project refs).
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Minesweeper.Api/Minesweeper.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
# Records DB lives on a PVC; an initContainer seeds it from the bundled copy on first run.
ENV MINESWEEPER_DB_PATH=/data/Minesweeper.db
VOLUME /data
EXPOSE 8080

ENTRYPOINT ["dotnet", "Minesweeper.Api.dll"]
