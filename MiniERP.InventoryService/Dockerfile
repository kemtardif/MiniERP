#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MiniERP.InventoryService/MiniERP.InventoryService.csproj", "MiniERP.InventoryService/"]
COPY ["CommonLib/CommonLib.csproj", "CommonLib/"]
RUN dotnet restore "MiniERP.InventoryService/MiniERP.InventoryService.csproj"
COPY . .
WORKDIR "/src/MiniERP.InventoryService"
RUN dotnet build "MiniERP.InventoryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MiniERP.InventoryService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniERP.InventoryService.dll"]