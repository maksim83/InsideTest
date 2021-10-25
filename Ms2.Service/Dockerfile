#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Ms2.Service/Ms2.Service.csproj", "Ms2.Service/"]
RUN dotnet restore "Ms2.Service/Ms2.Service.csproj"
COPY . .
WORKDIR "/src/Ms2.Service"
RUN dotnet build "Ms2.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ms2.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ms2.Service.dll"]