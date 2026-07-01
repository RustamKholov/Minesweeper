# Build the Blazor WebAssembly app to static files, serve with nginx. Build context is
# the repo root (Web references Core via a project ref).
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Minesweeper.Web/Minesweeper.Web.csproj -c Release -o /app/publish

FROM nginxinc/nginx-unprivileged:1.27-alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY Minesweeper.Web/nginx-default.conf /etc/nginx/conf.d/default.conf
EXPOSE 8080
