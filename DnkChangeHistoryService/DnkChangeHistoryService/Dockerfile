#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG ACR_PATH=mcr.microsoft.com
ARG HOST_IMAGE=dotnet/aspnet:8.0.13

FROM ${ACR_PATH}/${HOST_IMAGE} AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0.406 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DnkChangeHistoryService/DnkChangeHistoryService.csproj", "DnkChangeHistoryService/"]
COPY ["DnkChangeHistoryService.Tests/DnkChangeHistoryService.Tests.csproj", "DnkChangeHistoryService.Tests/"] 
ENV NUGET_CERT_REVOCATION_MODE=offline
RUN dotnet restore "./DnkChangeHistoryService/DnkChangeHistoryService.csproj"
RUN dotnet restore "./DnkChangeHistoryService.Tests/DnkChangeHistoryService.Tests.csproj"
COPY . .
WORKDIR "/src/DnkChangeHistoryService.Tests"
RUN dotnet build "DnkChangeHistoryService.Tests.csproj" -c Release -o /app 
RUN dotnet test "DnkChangeHistoryService.Tests.csproj" --logger "trx;LogFileName=webapplication1.trx" 

WORKDIR "/src/DnkChangeHistoryService"
RUN dotnet build "./DnkChangeHistoryService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DnkChangeHistoryService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DnkChangeHistoryService.dll"]