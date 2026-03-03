# Development Dockerfile for UrlShortenerApi

# Use the official .NET 8 SDK image for development
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev

# Set the working directory
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./UrlShortenerApi/*.csproj ./UrlShortenerApi/
RUN dotnet restore ./UrlShortenerApi/UrlShortenerApi.csproj

# Copy the rest of the source code
COPY . ./

# Expose the port the app listens on
EXPOSE 7102

# Set environment variables for development
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:7102
ENV ASPNETCORE_URLS=http://+:7102

# Enable hot reload and file watcher for development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true

# Install development certificates (optional for HTTPS, can be omitted if not needed)
RUN dotnet dev-certs https --clean
RUN dotnet dev-certs https --trust

# Create common directories that the application might need
RUN mkdir -p /app/UrlShortenerApi/logs /app/UrlShortenerApi/data

# Set proper permissions for the application directory
RUN chown -R app:app /app || true

# Start the application with hot reload
CMD ["dotnet", "watch", "run", "--project", "UrlShortenerApi/UrlShortenerApi.csproj", "--urls", "http://0.0.0.0:7102"]