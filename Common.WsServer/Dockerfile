#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Common.WsServer/Common.WsServer.Api.csproj", "Common.WsServer/"]
COPY ["Common.Models/Common.Models.csproj", "Common.Models/"]
COPY ["Common.Infrastructure/Common.Infrastructure.csproj", "Common.Infrastructure/"]
RUN dotnet restore "Common.WsServer/Common.WsServer.Api.csproj"
COPY . .
WORKDIR "/src/Common.WsServer"
RUN dotnet build "Common.WsServer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Common.WsServer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Common.WsServer.Api.dll"]