FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["SA.Web.Server/SA.Web.Server.csproj", "SA.Web.Server/"]
RUN dotnet restore "SA.Web.Server/SA.Web.Server.csproj"
COPY . .
WORKDIR "/src/SA.Web.Server"
RUN dotnet build "SA.Web.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SA.Web.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SA.Web.Server.dll"]