FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Quartile.Tests/Quartile.Tests.csproj", "src/Quartile.Tests/"]
COPY ["src/Quartile.Application/Quartile.Application.csproj", "src/Quartile.Application/"]
COPY ["src/Quartile.Domain/Quartile.Domain.csproj", "src/Quartile.Domain/"]
COPY ["src/Quartile.Infrastructure/Quartile.Infrastructure.csproj", "src/Quartile.Infrastructure/"]
RUN dotnet restore "./src/Quartile.Tests/./Quartile.Tests.csproj"
COPY . .
WORKDIR "/src/src/Quartile.Tests"
RUN dotnet build "./Quartile.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS test
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Quartile.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Quartile.Tests.dll"]