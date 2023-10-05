FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 81

ENV ASPNETCORE_URLS=https://+:443;http://+:80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["PurchaseToken.API/PurchaseToken.API.csproj", "PurchaseToken.API/"]
RUN dotnet restore "PurchaseToken.API/PurchaseToken.API.csproj"
COPY . .
WORKDIR "/src/PurchaseToken.API"
RUN dotnet build "PurchaseToken.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "PurchaseToken.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PurchaseToken.API.dll"]
