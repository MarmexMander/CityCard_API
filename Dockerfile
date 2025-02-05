FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update -yq && apt-get upgrade -yq
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
RUN apt-get update -yq && apt-get upgrade -yq
COPY ["CityCard_API.csproj", "./"]
RUN dotnet restore "CityCard_API.csproj"
COPY . .
RUN dotnet build "CityCard_API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "CityCard_API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#TODO: Remove next line from prod. Separate dev and rel dockerfiles
COPY --from=build /src/ /src/
ENTRYPOINT ["dotnet", "CityCard_API.dll"]
