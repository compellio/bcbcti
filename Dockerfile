FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BCBCTI.csproj", "./"]
RUN dotnet restore "BCBCTI.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "./BCBCTI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BCBCTI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#TODO define defaults to set disk storage as default and use /data
ENTRYPOINT ["dotnet", "BCBCTI.dll"]
