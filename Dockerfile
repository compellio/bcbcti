FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN mkdir -p /data && chown $APP_UID /data
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
VOLUME /data

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
ENV Storage__Provider=File \
    Storage__ProviderOptions__BaseFolder=/data
ENTRYPOINT ["dotnet", "BCBCTI.dll"]
