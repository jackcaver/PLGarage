FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build

COPY ["GameServer/GameServer.csproj", "GameServer/"]
COPY . .

RUN dotnet publish GameServer -c Release -o /build/publish/ --self-contained -p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS runtime

RUN apt-get update && apt-get install -y curl

COPY --from=build /build/publish /GameServer
COPY --from=build /build/docker-entrypoint.sh /GameServer

RUN mkdir /workdir
RUN chmod +x /GameServer/docker-entrypoint.sh

ENTRYPOINT ["/GameServer/docker-entrypoint.sh"]
