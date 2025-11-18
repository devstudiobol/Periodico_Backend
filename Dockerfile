# ----- Etapa 1: Compilación (Build) -----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia solo el archivo .csproj y restaura
# (Busca un archivo que termine en .csproj en esta carpeta)
COPY *.csproj .
RUN dotnet restore

# Copia todo el resto del código y compila
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ----- Etapa 2: Final (Runtime) -----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# El .dll tendrá el mismo nombre que tu proyecto (PeriodicoUpdate.dll)
ENTRYPOINT ["dotnet", "PeriodicoUpdate.dll"]