# Stage 1: Compilation
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy solution and project files to restore dependencies
COPY src/backend/SkyRoute.slnx ./
COPY src/backend/SkyRoute.Api/SkyRoute.Api.csproj ./SkyRoute.Api/
COPY src/backend/SkyRoute.Application/SkyRoute.Application.csproj ./SkyRoute.Application/
COPY src/backend/SkyRoute.Infrastructure/SkyRoute.Infrastructure.csproj ./SkyRoute.Infrastructure/
COPY src/backend/SkyRoute.Domain/SkyRoute.Domain.csproj ./SkyRoute.Domain/

RUN dotnet restore

# Copy all the rest of the code and compile
COPY src/backend/ ./
RUN dotnet publish SkyRoute.Api/SkyRoute.Api.csproj -c Release -o out

# Stage 2: Production Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the container's internal port
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "SkyRoute.Api.dll"]