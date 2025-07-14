FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Quartile.Api/Quartile.Api.csproj", "src/Quartile.Api/"]
COPY ["src/Quartile.Application/Quartile.Application.csproj", "src/Quartile.Application/"]
COPY ["src/Quartile.Domain/Quartile.Domain.csproj", "src/Quartile.Domain/"]
COPY ["src/Quartile.Infrastructure/Quartile.Infrastructure.csproj", "src/Quartile.Infrastructure/"]
RUN dotnet restore "src/Quartile.Api/Quartile.Api.csproj"
COPY . .
WORKDIR "/src/src/Quartile.Api"
RUN dotnet build "Quartile.Api.csproj" --configuration Release --no-restore --nologo --output /app/build 

FROM build AS publish
RUN dotnet publish "Quartile.Api.csproj" --configuration Release --no-restore --nologo --output /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Quartile.Api.dll"]