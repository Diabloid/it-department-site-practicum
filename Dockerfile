# ==========================================
# Етап 1: Збірка застосунку (Multi-stage Build)
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копіюємо проєктні файли для оптимізації кешування Docker шарів
COPY it-department-site.slnx .
COPY it_department_site/it_department_site.csproj it_department_site/
COPY ItDepartment.Tests/ItDepartment.Tests.csproj ItDepartment.Tests/

# Відновлення NuGet-пакетів
RUN dotnet restore it_department_site/it_department_site.csproj

# Копіюємо всі вихідні файли проєкту
COPY . .

# Компіляція та публікація у релізній конфігурації
WORKDIR /src/it_department_site
RUN dotnet publish -c Release -o /app/publish --no-restore

# ==========================================
# Етап 2: Фінальний образ для запуску (Runtime)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Запуск від імені вбудованого непривілейованого користувача app для безпеки
USER app

# Копіюємо скомпільовані бінарники з етапу збірки
COPY --from=build /app/publish .

# Відкриваємо стандартний порт 8080
EXPOSE 8080

# Підтримка динамічного порту Render ($PORT) та стандартного ASPNETCORE_HTTP_PORTS
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "it_department_site.dll"]
