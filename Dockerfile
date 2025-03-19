FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
# Указываем порт 80 в качестве URL для Kestrel
ENV ASPNETCORE_URLS=http://+:80
# Объявляем порт 80 для внешних подключений
EXPOSE 80

# Этап сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BitfinexAsp.Application/BitfinexAsp.Application.csproj", "BitfinexAsp.Application/"]
COPY ["BitfinexAsp.Web/BitfinexAsp.Web.csproj", "BitfinexAsp.Web/"]

RUN dotnet restore "./BitfinexAsp.Web/BitfinexAsp.Web.csproj"
COPY . .
WORKDIR "/src/BitfinexAsp.Web"
RUN dotnet build "./BitfinexAsp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации приложения
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BitfinexAsp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап — готовый контейнер
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BitfinexAsp.Web.dll"]
