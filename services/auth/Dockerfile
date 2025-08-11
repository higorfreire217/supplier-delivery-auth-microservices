# Use the official .NET runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY app/app.csproj ./app/
RUN dotnet restore ./app/app.csproj

# Copy everything and build
COPY app/ ./app/
WORKDIR /src/app
RUN dotnet publish -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose default ASP.NET port
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "app.dll"]