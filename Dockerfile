# 1. بيئة التشغيل الأساسية (الران تايم)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. بيئة البناء (البيلد)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# نسخ ملفات المشروع وعمل Restore (هتحتاج تتأكد إن مسارات واسماء المشاريع متطابقة معاك)
COPY ["src/UrlShortener.Api/UrlShortener.Api.csproj", "src/UrlShortener.Api/"]
COPY ["src/UrlShortener.Application/UrlShortener.Application.csproj", "src/UrlShortener.Application/"]
COPY ["src/UrlShortener.Domain/UrlShortener.Domain.csproj", "src/UrlShortener.Domain/"]
COPY ["src/UrlShortener.Infrastructure/UrlShortener.Infrastructure.csproj", "src/UrlShortener.Infrastructure/"]
RUN dotnet restore "src/UrlShortener.Api/UrlShortener.Api.csproj"

# نسخ باقي الكود وعمل بيلد
COPY . .
WORKDIR "/src/src/UrlShortener.Api"
RUN dotnet build "UrlShortener.Api.csproj" -c Release -o /app/build

# 3. النشر (البابليش)
FROM build AS publish
RUN dotnet publish "UrlShortener.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. الصورة النهائية اللي هتشتغل
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlShortener.Api.dll"]