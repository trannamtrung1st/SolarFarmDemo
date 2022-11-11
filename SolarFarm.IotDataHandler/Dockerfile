FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SolarFarm.IotDataHandler/SolarFarm.IotDataHandler.csproj", "SolarFarm.IotDataHandler/"]
RUN dotnet restore "SolarFarm.IotDataHandler/SolarFarm.IotDataHandler.csproj"
COPY . .
WORKDIR "/src/SolarFarm.IotDataHandler"
RUN dotnet build "SolarFarm.IotDataHandler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SolarFarm.IotDataHandler.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

CMD sleep 25s; dotnet SolarFarm.IotDataHandler.dll
