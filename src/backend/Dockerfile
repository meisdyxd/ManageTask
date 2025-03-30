FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ManageTask.API/ManageTask.API.csproj", "ManageTask.API/"]
COPY ["ManageTask.Contracts/ManageTask.Contracts.csproj", "ManageTask.Contracts/"]
COPY ["ManageTask.Infrastructure/ManageTask.Infrastructure.csproj", "ManageTask.Infrastructure/"]
COPY ["ManageTask.Domain/ManageTask.Domain.csproj", "ManageTask.Domain/"]
COPY ["ManageTask.Common/ManageTask.Common.csproj", "ManageTask.Common/"]
COPY ["ManageTask.Application/ManageTask.Application.csproj", "ManageTask.Application/"]
RUN dotnet restore "./ManageTask.API/ManageTask.API.csproj"
COPY . .
WORKDIR "/src/ManageTask.API"
RUN dotnet build "./ManageTask.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ManageTask.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ManageTask.API.dll"]