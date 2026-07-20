# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj files and restore as separate layers
COPY .././it_department_site.csproj .

RUN dotnet restore it_department_site.csproj

# Copy only relevant source;
#COPY Backoffice ./Backoffice

RUN dotnet publish it_department_site.csproj -c Release -o /app/publish --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

RUN adduser --disabled-password --gecos "" appuser
USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "it_department_site.dll"]
