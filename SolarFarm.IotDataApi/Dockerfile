FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SolarFarm.IotDataApi/SolarFarm.IotDataApi.csproj", "SolarFarm.IotDataApi/"]
RUN dotnet restore "SolarFarm.IotDataApi/SolarFarm.IotDataApi.csproj"
COPY . .
WORKDIR "/src/SolarFarm.IotDataApi"
RUN dotnet build "SolarFarm.IotDataApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SolarFarm.IotDataApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

CMD sleep 15s; dotnet SolarFarm.IotDataApi.dll
