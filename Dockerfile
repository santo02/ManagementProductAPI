FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Tambahkan NuGet source untuk preview packages
RUN dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org || true

# Copy project file
COPY ["ManagementProduct.csproj", "."]

# Restore dengan flag allow preview
RUN dotnet restore --source https://api.nuget.org/v3/index.json

# Copy semua file
COPY . .

# Publish (tanpa --no-restore agar restore ulang jika perlu)
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ManagementProduct.dll"]