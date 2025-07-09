FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build

COPY ["GameServer/GameServer.csproj", "GameServer/"]
COPY . .

RUN dotnet publish GameServer -c Release -o /build/publish/ --self-contained -p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime:8.0.0-bookworm-slim AS runtime

COPY --from=build /build/publish /GameServer
COPY --from=build /build/docker-entrypoint.sh /GameServer

RUN mkdir /workdir
RUN chmod +x /GameServer/docker-entrypoint.sh

ENTRYPOINT ["/GameServer/docker-entrypoint.sh"]
