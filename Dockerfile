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

COPY ["OzonProductsApi.Persistence/OzonProductsApi.Persistence.csproj", "OzonProductsApi.Persistence/"]
COPY ["OzonProductsApi.Application/OzonProductsApi.Application.csproj", "OzonProductsApi.Application/"]
COPY ["OzonProductsApi.Web/OzonProductsApi.Web.csproj", "OzonProductsApi.Web/"]
RUN dotnet restore "./OzonProductsApi.Web/OzonProductsApi.Web.csproj"
COPY . .
WORKDIR "/src/OzonProductsApi.Web"
RUN dotnet build "./OzonProductsApi.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации приложения
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OzonProductsApi.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап — готовый контейнер
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Копируем ozonCategoryTree.json в /app/Data/
RUN mkdir -p /app/Data
COPY --from=publish src/OzonProductsApi.Web/bin/Data/ozonCategoryTree.json /app/Data/ozonCategoryTree.json

ENTRYPOINT ["dotnet", "OzonProductsApi.Web.dll"]
